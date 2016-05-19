using EshopMVC.Controllers.Cart;
using EshopMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace EshopMVC
{
    public class AppUser
    {
        private static IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        private static UserManager<ApplicationUser> UserManager
        {
            get
            {
                return new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            }
        }

        //private object _cart;

        //public object Cart
        //{
        //    get { return _cart; }
        //    set { _cart = value; }
        //}

        private static string _userName;

        public static string UserName
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return HttpContext.Current.User.Identity.Name;
                }
                return _userName;
            }
        }

        public static bool SignIn(string userName, string Password)
        {
            var user = UserManager.Find(userName, Password);
            if (user != null)
            {
                SignInAsync(user, false);

                using (var dbCtx = new DB_9FCCB1_eshopEntities())
                {
                    if (dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id) == null)
                    {
                        dbCtx.ShoppingCart.Add(
                            new ShoppingCart
                            {
                                UserId = user.Id
                            });
                        dbCtx.SaveChanges();
                    }
                }

                var sessionCart = (SessionCart) HttpContext.Current.Session["Cart"];
                if (sessionCart != null) //todo: merge carts, or allow multiple carts for user?
                {
                    var dbCart = new DbCart(user.UserName);
                    dbCart.Empty();
                    foreach (CartItem item in sessionCart.LoadItems())
                    {
                        dbCart.AddItem(item.ProductId, item.Quantity);
                    }
                }
                HttpContext.Current.Session.Remove("Cart");
                return true;
            }
            return false;
        }

        private static void SignInAsync(ApplicationUser user, bool isPersistent) //todo: async?
        {
            //info:
            //The key methods are SignIn() and SignOut() on the AuthenticationManager, which create or delete the application cookies on the executing request. 
            //SignIn() takes an Identity object that includes any claims you have assigned to it. 
            //This identity is what you also get back once the user is logged in and you look at Context.User.Identity later to check for authorization.
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() {IsPersistent = isPersistent}, identity);
            //if (identity.IsAuthenticated)
            //{
            //    _userName = identity.Name;
            //}
        }
    }
}
