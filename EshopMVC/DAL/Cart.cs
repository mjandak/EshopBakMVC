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

namespace EshopMVC.DAL
{
    public abstract class Cart
    {
        public abstract CartItem[] LoadItems();

        public abstract void AddItem(int id, int quantity);

        public abstract void Empty();
    }

    public class CartItemInfo
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }

    public class SessionCart : Cart
    {
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

    public class DbCart : Cart
    {
        public UserManager<ApplicationUser> UserManager { get; private set; }

        private ShoppingCart _cart;

        public override CartItem[] LoadItems()
        {
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                dbCtx.ShoppingCart.Attach(_cart);
                var products = dbCtx
                    .CartProduct
                    .Select(cp => cp.Product).ToArray();
                var cartItems = products
                    .Select(
                        p => new CartItem
                        {
                            Price = p.price,
                            ProductId = p.id,
                            Quantity = _cart.CartProduct.First(i => i.ProductId == p.id).Quantity,
                            Title = p.title
                        }
                    ).ToArray();
                return cartItems;
            }
        }

        public DbCart(string UserName)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = UserManager.FindByName(UserName);
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                _cart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id);

                if (_cart != null) return;
                _cart = new ShoppingCart();
                _cart.UserId = user.Id;
                dbCtx.ShoppingCart.Add(_cart);
                dbCtx.SaveChanges();
            }
        }

        public override void AddItem(int id, int quantity)
        {
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                dbCtx.ShoppingCart.Attach(_cart);
                var sameItem = _cart.CartProduct.FirstOrDefault(i => i.ProductId == id);
                if (sameItem == null)
                {
                    _cart.CartProduct.Add(
                        new CartProduct
                        {
                            CartId = _cart.Id,
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
                dbCtx.ShoppingCart.Attach(_cart);
                _cart.CartProduct.Clear();
                dbCtx.SaveChanges();
            }
        }
    }
}
