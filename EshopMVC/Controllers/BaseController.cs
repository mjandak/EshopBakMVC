using EshopMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EshopMVC.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //todo: see http://stackoverflow.com/questions/658747/how-do-i-maintain-modelstate-errors-when-using-redirecttoaction
            //preserve login error message after redirect
            if (TempData["ModelState"] != null && !ModelState.Equals(TempData["ModelState"]))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

            base.OnActionExecuted(filterContext);
        }
	}
}