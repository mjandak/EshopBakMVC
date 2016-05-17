using EshopMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace EshopMVC.DAL
{
    public class AppUser
    {
        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        private object _cart;

        public object Cart
        {
            get { return _cart; }
            set { _cart = value; }
        }

        public AppUser()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        public void SignIn(string UserName, string Password)
        {
            var user = UserManager.Find(UserName, Password);
            if (user != null)
            {
                SignInAsync(user, false);
                //RedirectToAction("Home", "Index");
                //return RedirectToLocal(returnUrl);

                var sessionCart = (SessionCart) HttpContext.Current.Session["Cart"];
                if (sessionCart != null)
                {
                    using (var dbCtx = new DB_9FCCB1_eshopEntities())
                    {
                        var dbCart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id);
                        if (dbCart == null)
                        {
                            var dbCartWrapper = new DbCart(user.UserName);
                            foreach (CartItem item in sessionCart.Items)
                            {
                                dbCartWrapper.AddItem(item.ProductId, item.Quantity);
                            }

                            var x = dbCartWrapper.Items;

                            //dbCartWrapper.Save();

                            //dbCtx.ShoppingCart.Add(sessionCart);
                            //dbCtx.SaveChanges();
                        }
                        else
                        {
                            dbCart.CartProduct.Clear();
                            dbCtx.SaveChanges();
                            // dbCart.CartProduct = sessionCart.CartProduct; //TODO: merge carts
                            dbCtx.SaveChanges();
                        }
                    }
                }
                HttpContext.Current.Session.Remove("Cart");
            }
        }

        private void SignInAsync(ApplicationUser user, bool isPersistent)
        {
            //The key methods are SignIn() and SignOut() on the AuthenticationManager, which create or delete the application cookies on the executing request. 
            //SignIn() takes an Identity object that includes any claims you have assigned to it. 
            //This identity is what you also get back once the user is logged in and you look at Context.User.Identity later to check for authorization.
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() {IsPersistent = isPersistent}, identity);
            //if (identity.IsAuthenticated)
            //{
            //    ViewBag.UserName = identity.Name;
            //}

        }
    }
}
