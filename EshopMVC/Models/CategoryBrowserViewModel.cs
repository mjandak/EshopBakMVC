using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EshopMVC.Models
{
    public class CategoryBrowserViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}