using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EshopMVC.Models.Cart
{
    public class CartViewModel
    {
        public CartItemViewModel[] Items { get; private set; }
        
        public decimal Total { get; private set; }

        public CartViewModel()
        {

        }

        public CartViewModel(IQueryable<CartProduct> items)
        {
            Items =  items.Select(i => new CartItemViewModel(i)).ToArray();
            Total = items.Sum(i => i.Product.price*i.Quantity);
        }
    }
}