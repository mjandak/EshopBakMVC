using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Summary description for ShoppingCart
/// </summary>

namespace EshopMVC.Models
{
    [Serializable]
    public class ShoppingCart
    {
        private Dictionary<int, CartItem> cartItems = new Dictionary<int, CartItem>();

        [Key]
        public int Id { get; set; }

        public Dictionary<int, CartItem>.ValueCollection CartItems // vsechny produkty v kosiku
        {
            get { return cartItems.Values; }
        }

        public decimal Total // celkova cena vsech produktu v kosiku
        {
            get
            {
                decimal sum = 0;
                foreach (CartItem item in cartItems.Values)
                    sum += item.Price * item.Quantity;
                return sum;
            }
        }

        public void AddItem(int id, string name, decimal price, int quantity) // pridej do kosiku novy produkt
        {
            if (cartItems.ContainsKey(id)) //je produkt s danym id jiz v kosiku?
            {
                cartItems[id].Quantity += quantity;
            }
            else
            {
                //cartItems.Add(id, new CartItem(id, name, price, quantity));
                cartItems.Add(
                    id, 
                    new CartItem { Id = id, Title = name, Price = price, Quantity = quantity }
                    );
            }
        }

        public void RemoveItem(int id) //odstran produkt z kosiku
        {
            CartItem item = (CartItem)cartItems[id];
            if (item == null)
            {
                return;
            }
            cartItems.Remove(id);
        }

        public void ItemQuantityChange(int id, int quantity) //zmen mnozstvi nejakeho produktu v kosiku
        {
            if (quantity == 0)
            {
                cartItems.Remove(id);
            }
            else
            {
                if (quantity < 0)
                {
                    return;
                }
                else
                {
                    CartItem item = cartItems[id];
                    item.Quantity = quantity;
                }
            }
        }

        public void Empty() //vyprazdni kosik
        {
            cartItems.Clear();
        }

    } 
}
