using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EshopMVC;

namespace EshopMVC.Models.Order
{
    public class OrderSummaryViewModel
    {
        public OrderSummaryViewModel()
        {
        }

        public OrderSummaryViewModel(EshopMVC.Order order)
        {
            Id = order.Id;
            CreateDate = order.CreateDate;
            State = "TODO";
            TotalPrice = order.OrderProduct.Sum(p => p.Quantity*p.Product.price);
        }

        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string State { get; set; }
        public decimal TotalPrice { get; set; }
    }
}