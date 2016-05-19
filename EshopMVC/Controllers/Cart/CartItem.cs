using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EshopMVC.Controllers.Cart
{
    public class CartItem
    {
        public int ProductId
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }
    }
}