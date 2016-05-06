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

        public CartViewModel(ShoppingCart cart)
        {
            Items =  cart.CartItem.Select(i => new CartItemViewModel(i)).ToArray();
            Total = cart.CartItem.Sum(i => i.Price*i.Quantity);
        }
    }
}