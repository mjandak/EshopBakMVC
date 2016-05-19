using EshopMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace EshopMVC.Controllers.Cart
{
    public abstract class CartStrategy
    {
        public abstract CartItem[] LoadItems();

        public abstract void AddItem(int id, int quantity);

        public abstract void Empty();
    }

    public class SessionCart : CartStrategy
    {
        public class CartItemInfo
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
        }

        List<CartItemInfo> _itemsInfo = new List<CartItemInfo>();

        List<CartItem> _items = new List<CartItem>();

        public override CartItem[] LoadItems()
        {
            var ids = _itemsInfo.Select(i => i.Id).ToArray();

            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                var cartItems = dbCtx.Product
                                   .Where(p => ids.Contains(p.id))
                                   .ToArray()
                                   .Select(
                                       p => new CartItem()
                                       {
                                           Price = p.price,
                                           ProductId = p.id,
                                           Quantity = _itemsInfo.First(i => i.Id == p.id).Quantity,
                                           Title = p.title
                                       }
                                   ).ToArray();
                return cartItems;
            }
        }

        public override void AddItem(int id, int quantity)
        {
            var sameItem = _itemsInfo.FirstOrDefault(i => i.Id == id);
            if (sameItem == null)
            {
                _itemsInfo.Add(new CartItemInfo { Id = id, Quantity = quantity });
                return;
            }
            sameItem.Quantity += quantity;
        }

        public override void Empty()
        {
            _itemsInfo.Clear();
        }
    }

    public class DbCart : CartStrategy
    {
        private string _userId;

        public override CartItem[] LoadItems()
        {
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                var cart = dbCtx
                    .ShoppingCart.Where(c => c.UserId == _userId)
                    .Include(c => c.CartProduct.Select(cp => cp.Product))
                    .First();
                var cartItems = cart.CartProduct
                    .Select(
                        cp => new CartItem
                        {
                            Price = cp.Product.price,
                            ProductId = cp.ProductId,
                            Quantity = cp.Quantity,
                            Title = cp.Product.title
                        }
                    ).ToArray();
                return cartItems;
            }
        }

        /// <summary>
        /// Creates a new cart in db, if the user doesn't have one already. 
        /// </summary>
        /// <param name="UserName"></param>
        public DbCart(string UserName)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = UserManager.FindByName(UserName);
            _userId = user.Id;
        }

        public override void AddItem(int id, int quantity)
        {
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                var cart = dbCtx.ShoppingCart.First(c => c.UserId == _userId);
                var sameItem = cart.CartProduct.FirstOrDefault(i => i.ProductId == id);
                if (sameItem == null)
                {
                    cart.CartProduct.Add(
                        new CartProduct
                        {
                            //CartId = _cart.Id,
                            ProductId = id,
                            Quantity = quantity
                        });
                }
                else
                {
                    sameItem.Quantity += quantity;
                }
                dbCtx.SaveChanges();
            }
        }

        public override void Empty()
        {
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                dbCtx.ShoppingCart.First(c => c.UserId == _userId).CartProduct.Clear();
                dbCtx.SaveChanges();
            }
        }
    }
}
