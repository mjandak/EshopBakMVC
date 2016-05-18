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
        public abstract IEnumerable<CartItem> Items { get; }

        public static Cart GetInstatnce()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return new DbCart(HttpContext.Current.User.Identity.Name);
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
        private bool _modified;

        public UserManager<ApplicationUser> UserManager { get; private set; }

        private ShoppingCart _cart;

        //List<CartProduct> _itemsInfo = new List<CartProduct>();

        public override IEnumerable<CartItem> Items
        {
            get
            {
                var ids = _cart.CartProduct.Select(i => i.ProductId).ToArray();
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
                                               Quantity = _cart.CartProduct.First(i => i.ProductId == p.id).Quantity,
                                               Title = p.title
                                           }
                                       ).ToArray();
                    return cartItems;
                }

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
            ApplicationUser user = UserManager.FindByName(UserName);

            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {    
                _cart = dbCtx.ShoppingCart
                    .Where(c => c.UserId == user.Id)
                    .Include(c => c.CartProduct)
                    .FirstOrDefault(c => c.UserId == user.Id);

                if (_cart == null)
                {
                    _cart = new ShoppingCart();
                    _cart.UserId = user.Id;
                    dbCtx.ShoppingCart.Add(_cart);
                    dbCtx.SaveChanges();
                }
            }
        }

        public override void AddItem(int id, int quantity)
        {
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
                return;
            }
            sameItem.Quantity += quantity;
        }

        public override void Save()
        {
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                //dbCtx.CartProduct.Where(cp => cp.CartId == _cart.Id).
                //var deleteCartProduct = new CartProduct() { CartId = _cart.Id };
                //dbCtx.CartProduct.Attach(deleteCartProduct);
                //dbCtx.CartProduct.delete
                var cartProducts = dbCtx.ShoppingCart.First(c => c.Id == _cart.Id).CartProduct;
                cartProducts.Clear();
                dbCtx.SaveChanges();


                foreach (var item in _cart.CartProduct)
                {
                    cartProducts.Add(item);
                }
                dbCtx.SaveChanges();
            }
        }

        public override void Empty()
        {
            //_itemsInfo.Clear();
            _cart.CartProduct.Clear();
        }
    }
}
