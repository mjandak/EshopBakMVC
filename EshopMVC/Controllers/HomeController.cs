using EshopMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EshopMVC.Controllers
{
    public class HomeController : Controller
    {
        private DB_9FCCB1_eshopEntities db = new DB_9FCCB1_eshopEntities();

        //
        // GET: /Home/
        public ActionResult Index(int? CatId, int? PageStart)
        {
            using (db)
            {
                var model = new CategoryBrowserViewModel();
                int pageStart = PageStart ?? 1;

                ViewBag.CatId = CatId;
                ViewBag.Next = pageStart + 9;
                ViewBag.Prev = Math.Max(pageStart - 9, 1);

                if (CatId == null)
                {
                    var products = db.Product.Where(p => p.special)
                        .OrderBy(p => p.id)
                        .Skip(pageStart)
                        .Take(9);

                    var topCtgrs = db.Category.Where(c => c.parent_id == 0).ToArray();
                    model.Categories = topCtgrs.Select(c => new CategoryViewModel(c)).ToArray();
                    model.Products = products.ToArray();
                    
                    return View(model);
                }
                else
                {
                    CreateTreeModel2(CatId.Value, null);
                    model.Categories = stack.Peek();

                    List<Category> childCtgrs = db.Category.Where(c => c.parent_id == CatId).ToList();
                    var subCtgrIds = childCtgrs.Traverse(GetChildCtgrs).ToList().Select(c => c.id);

                    var products = db.Product.Where(
                        p => p.Category.Any(
                            c => subCtgrIds.Contains(c.id) || c.id == CatId.Value))
                            .Distinct()
                            .OrderBy(p => p.id)
                            .Skip(pageStart)
                            .Take(9);

                    model.Products = products.ToList();
                }

                return View(model); 
            }
        }

        private IEnumerable<Category> GetChildCtgrs(Category ctgr)
        {
            return db.Category.Where(c => c.parent_id == ctgr.id);
        }

        private Stack<IEnumerable<CategoryViewModel>> stack = new Stack<IEnumerable<CategoryViewModel>>();

        private void CreateTreeModel2(int ctgrId, int? previousCtgrId)
        {
            var childCtgrs = db.Category.Where(c => c.parent_id == ctgrId).ToArray()
                .Select(c => new CategoryViewModel(c)).ToArray();
            if (previousCtgrId != null)
            {
                var prevCtgr = childCtgrs.First(c => c.Id == previousCtgrId);
                prevCtgr.Children = stack.Peek();
            }
            stack.Push(childCtgrs);
            if (ctgrId == 0) return;
            CreateTreeModel2(db.Category.First(c => c.id == ctgrId).parent_id, ctgrId);
        }

        //private CategoryViewModel CreateTree(int nodeParentId, int nodeId, List<CategoryViewModel> nodeChilds)
        //{
        //    CategoryViewModel parent;
        //    if (nodeParentId != 0)
        //    {
        //        parent = new CategoryViewModel(db.Category.First(c => c.id == nodeParentId));
        //        parent.Children = db.Category.Where(c => c.parent_id == nodeParentId).ToList().Select(c => new CategoryViewModel(c)).ToList();
        //        parent.Children.First(c => c.Id == nodeId).Children = nodeChilds;
        //        return CreateTree(parent.ParentId, parent.Id, parent.Children);
        //    }
        //    else
        //    {
        //        parent = new CategoryViewModel();
        //        parent.Children = db.Category.Where(c => c.parent_id == nodeParentId).ToList().Select(c => new CategoryViewModel(c)).ToList();
        //        parent.Children.First(c => c.Id == nodeId).Children = nodeChilds;
        //        return parent;
        //    }
        //}
	}
}