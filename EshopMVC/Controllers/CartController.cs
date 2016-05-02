using EshopMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EshopMVC.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                //new ApplicationDbContext().
            }
            else
            {
                var cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                cart.AddItem(id, "", 0, 0);
            }
            return View();
        }
	}
}