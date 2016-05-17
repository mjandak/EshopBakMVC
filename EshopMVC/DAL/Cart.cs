using EshopMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EshopMVC.DAL
{
    public abstract class Cart
    {
        public abstract IEnumerable<CartItem> Items { get; }

        public static Cart GetInstatnce()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return new DbCart();
            }
            var cart = (SessionCart)HttpContext.Current.Session["Cart"];
            if (cart == null)
            {
                HttpContext.Current.Session["Cart"] = new SessionCart();
            }
            return (SessionCart)HttpContext.Current.Session["Cart"];
        }

        public abstract void AddItem(int id, int quantity);

        public abstract void Save();

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

        public override IEnumerable<CartItem> Items
        {
            get
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
        }

        public SessionCart()
        {
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

        public override void Save()
        {
            HttpContext.Current.Session["Cart"] = this;
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

        List<CartProduct> _itemsInfo = new List<CartProduct>();

        public override IEnumerable<CartItem> Items
        {
            get
            {
                return _cart.CartProduct.Select(cp => new CartItem
                {
                    ProductId = cp.ProductId, 
                    Quantity = cp.Quantity, 
                    Price = cp.Product.price, 
                    Title = cp.Product.title
                }).ToArray();
            }
        }

        public DbCart(string UserName)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                ApplicationUser user = UserManager.FindByName(UserName);
                _cart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id);
            }
        }

        public override void AddItem(int id, int quantity)
        {
            var sameItem = _cart.CartProduct.FirstOrDefault(i => i.ProductId == id);
            if (sameItem == null)
            {
                _cart.CartProduct.Add(new CartProduct { ProductId = id, Quantity = quantity });
                return;
            }
            sameItem.Quantity += quantity;
        }

        public override void Save()
        {
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                ApplicationUser user = UserManager.FindByName(HttpContext.Current.User.Identity.Name);
                _cart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id);
            }
        }

        public override void Empty()
        {
            _itemsInfo.Clear();
        }
    }
}
