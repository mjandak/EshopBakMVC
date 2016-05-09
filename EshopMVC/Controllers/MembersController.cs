using EshopMVC.Models;
using EshopMVC.Models.Members;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EshopMVC.Controllers
{
    public class MembersController : BaseController
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //
        // GET: /Users/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() 
                { 
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName  = model.LastName,
                    Street    = model.Street,
                    City      = model.City,
                    ZipCode   = model.ZipCode
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInAsync(user, isPersistent: false);
                    return View("RegisterSuccess");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model /*, string ReturnUrl*/)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.Find(model.UserName, model.Password);
                if (user != null)
                {
                    SignInAsync(user, false);
                    //RedirectToAction("Home", "Index");
                    //return RedirectToLocal(returnUrl);

                    var sessionCart = (ShoppingCart)Session["Cart"];
                    if (sessionCart != null)
                    {
                        using (var dbCtx = new DB_9FCCB1_eshopEntities())
                        {
                            var dbCart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id);
                            if (dbCart == null)
                            {
                                dbCtx.ShoppingCart.Add(sessionCart);
                                dbCtx.SaveChanges();
                            }
                            else
                            {
                                dbCart.CartItem = sessionCart.CartItem; //TODO: merge carts
                                dbCtx.SaveChanges();
                            }
                        }
                    }
                    Session.Remove("Cart");

                    return PartialView("_LoggedIn");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            //return RedirectToLocal(ReturnUrl);
            return PartialView("_Login", model);
        }

        private void SignInAsync(ApplicationUser user, bool isPersistent)
        {
            //The key methods are SignIn() and SignOut() on the AuthenticationManager, which create or delete the application cookies on the executing request. 
            //SignIn() takes an Identity object that includes any claims you have assigned to it. 
            //This identity is what you also get back once the user is logged in and you look at Context.User.Identity later to check for authorization.
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            if (identity.IsAuthenticated)
            {
                ViewBag.UserName = identity.Name;
            }
            
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
	}
}
