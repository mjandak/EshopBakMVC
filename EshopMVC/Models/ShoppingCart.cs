using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EshopMVC
{
    public partial class ShoppingCart
    {
        public void AddItem(CartProduct item)
        {
            var cartItem = CartProduct.FirstOrDefault(c => c.ProductId == item.ProductId);
            if (cartItem == null)
            {
                CartProduct.Add(item);
                return;
            }
            cartItem.Quantity += item.Quantity;
        }

        public void AddItems(IEnumerable<CartProduct> items)
        {
            foreach (CartProduct item in items)
            {
                this.AddItem(item);
            }
        }
    }
}