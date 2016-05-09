using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EshopMVC.Models
{
    public class CategoryBrowserViewModel
    {
        public CategoryBrowserViewModel()
        {
            OrderByOptions = new SelectList(
                new SelectItemModel[] { 
                    new SelectItemModel(0, "Newest"),
                    new SelectItemModel(1, "Cheapest"),
                    new SelectItemModel(2, "Most expensive")
                }, "Value", "Text");
        }

        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public IEnumerable<SelectListItem> OrderByOptions { get; set; }
    }

    public class SelectItemModel
    {
        public SelectItemModel(int value, string text)
        {
            Value = value;
            Text = text;
        }

        public int Value { get; set; }
        public string Text { get; set; }
    }
}