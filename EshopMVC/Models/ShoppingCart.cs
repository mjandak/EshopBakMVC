using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EshopMVC
{
    public partial class ShoppingCart
    {
        public void AddItem(CartItem item)
        {
            var cartItem = CartItem.FirstOrDefault(c => c.Id == item.Id);
            if (cartItem == null)
            {
                CartItem.Add(item);
                return;
            }
            cartItem.Quantity += item.Quantity;
        }

        public void AddItems(IEnumerable<CartItem> items)
        {
            foreach (CartItem item in items)
            {
                this.AddItem(item);
            }
        }
    }
}