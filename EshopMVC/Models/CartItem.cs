using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Summary description for CartItem
/// </summary>

namespace EshopMVC.Models
{
    [Serializable]
    public class CartItem
    {
        private int id;
        private string title;
        private decimal price;
        private int quantity;

        [Key]
        public int Id
        {
             get; set;
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

        //public CartItem(int Id, string Title, decimal Price, int Quantity)
        //{
        //    id = Id;
        //    title = Title;
        //    price = Price;
        //    quantity = Quantity;
        //}

    }
    
}