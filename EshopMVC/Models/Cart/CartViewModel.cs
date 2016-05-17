using EshopMVC.DAL;
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

        public CartViewModel(CartItemViewModel[] items)
        {
            //Items =  items.Select(i => new CartItemViewModel(i)).ToArray();
            Items = items;
            Total = items.Sum(i => i.Price*i.Quantity);
        }

        public CartViewModel(CartItem[] items)
        {
            Items = items.Select(i => new CartItemViewModel()
            {
                Price = i.Price,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Title = i.Title
            }).ToArray();
            Total = items.Sum(i => i.Price * i.Quantity);
        }
    }
}