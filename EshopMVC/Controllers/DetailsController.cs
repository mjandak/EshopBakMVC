using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EshopMVC.Controllers
{
    public class DetailsController : Controller
    {
        // GET: Details
        public ActionResult Index(int Id)
        {
            using (var db = new DB_9FCCB1_eshopEntities())
            {
                var product = db.Product.FirstOrDefault(p => p.id == Id);
                if (product == null)
                {
                    throw new Exception("Product not found.");
                }
                return View(product);
            }
        }

        public ActionResult Add(int ProductId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}