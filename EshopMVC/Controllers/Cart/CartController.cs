using EshopMVC.Models;
using EshopMVC.Models.Cart;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EshopMVC.Controllers.Cart
{
    public class CartController : BaseController
    {
        private CartStrategy Cart;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //info:
            //HttpContext is not available in the controller constructor
            //You could override the Initialize method where it will be accessible
            base.Initialize(requestContext);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Cart = new DbCart(User.Identity.Name);
                return;
            }
            var cart = (SessionCart)Session["Cart"];
            if (cart == null)
            {
                Session["Cart"] = new SessionCart();
            }
            Cart = (SessionCart)Session["Cart"];
        }

        public ActionResult Index()
        {
            var items = Cart.LoadItems();
            if (!items.Any()) //todo: IsEmpty
            {
                return View("Empty");
            }
            var model2 = new CartViewModel(items.ToArray());
            return View(model2);
        }

        public void Add(int id, int quantity)
        {
            Cart.AddItem(id, quantity);
        }

        public ActionResult Empty()
        {
            Cart.Empty();
            return View();
        }

        public ActionResult SaveChanges(IEnumerable<CartItemViewModel> items)
        {
            Cart.Empty();
            foreach (CartItemViewModel item in items)
            {
                Cart.AddItem(item.ProductId, item.Quantity); //todo: add collection
            }
            var model = new CartViewModel(Cart.LoadItems());
            return View("Index", model);
        }
    }
}
