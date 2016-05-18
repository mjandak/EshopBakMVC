using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EshopMVC;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ICollection<CartProduct> x;
            ShoppingCart sc;
            CartProduct item;
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                //dbCtx.ShoppingCart.First(c => c.Id == 1).CartProduct.Clear();

                //dbCtx.ShoppingCart.First(c => c.Id == 1).CartProduct.Add(
                //    new CartProduct { ProductId = 2, Quantity = 3 });

                sc = dbCtx.ShoppingCart.Where(c => c.Id == 1).Include("CartProduct").First();

                //dbCtx.SaveChanges();
            }

            item = new CartProduct
            {
                CartId = sc.Id, 
                ProductId = 4,
                Quantity = 3
            };
            sc.CartProduct.Add(item);

            using (var dbCtx2 = new DB_9FCCB1_eshopEntities())
            {
                try
                {
                    //dbCtx2.CartProduct.Attach(item);
                    //dbCtx2.ShoppingCart.Attach(sc);
                    dbCtx2.Entry(item).State = EntityState.Added;
                    dbCtx2.Entry(sc).State = EntityState.Modified;
                    dbCtx2.SaveChanges();
                    //var products = sc.CartProduct.Select(cp => cp.Product).ToArray();

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
