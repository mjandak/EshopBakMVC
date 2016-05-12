﻿using EshopMVC.Models;
using EshopMVC.Models.Cart;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EshopMVC.Controllers
{
    public class CartController : BaseController
    {
        public ActionResult Index()
        {
            IEnumerable<CartProduct> cart;
            using (var dbCtx = new DB_9FCCB1_eshopEntities())
            {
                if (User.Identity.IsAuthenticated)
                {
                    ApplicationUser user = UserManager.FindByName(User.Identity.Name);

                    cart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id).CartProduct;
                }
                else
                {
                    cart = (IEnumerable<CartProduct>)Session["Cart"];
                }

                if (cart == null || cart.Count() < 1)
                {
                    return View("Empty");
                }

                var model = new CartViewModel(cart.AsQueryable<CartProduct>());

                return View(model);
            }
        }

        public void Add(int id, int quantity)
        {
            var cartItem = new CartProduct { ProductId = id, Quantity = quantity };
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = UserManager.FindByName(User.Identity.Name);
                using (var dbCtx = new DB_9FCCB1_eshopEntities())
                {
                    var dbCart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id);
                    if (dbCart == null)
                    {
                        dbCart = new ShoppingCart() { UserId = user.Id };
                        dbCart.AddItem(cartItem);
                        dbCtx.ShoppingCart.Add(dbCart);
                        dbCtx.SaveChanges();
                    }
                    else
                    {
                        dbCart.AddItem(cartItem);
                        dbCtx.SaveChanges();
                    }
                }
            }
            else
            {
                var cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                    cart.AddItem(cartItem);
                    Session.Add("Cart", cart);
                }
                else
                {
                    cart.AddItem(cartItem);
                }
            }
        }

        public ActionResult Empty()
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = UserManager.FindByName(User.Identity.Name);
                using (var dbCtx = new DB_9FCCB1_eshopEntities())
                {
                    var dbCart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id);
                    if (dbCart == null)
                    {
                        throw new Exception("Should not get here."); //TODO
                    }
                    else
                    {
                        dbCtx.CartProduct.RemoveRange(dbCart.CartProduct);
                        dbCart.CartProduct.Clear();
                        dbCtx.SaveChanges();
                    }
                }
            }
            else
            {
                var cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    throw new Exception("Should not get here."); //TODO
                }
                else
                {
                    cart.CartProduct.Clear();
                }
            }
            return View();
        }

        //public ActionResult SaveChanges(IEnumerable<CartItemViewModel> items)
        //{
        //    var cartItems = items.Select(
        //        i => new CartProduct { ProductId = i.ProductId, Quantity = i.Quantity }
        //        ).Where(i => i.Quantity > 0).ToArray();

        //    ShoppingCart cart;
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ApplicationUser user = UserManager.FindByName(User.Identity.Name);
        //        using (var dbCtx = new DB_9FCCB1_eshopEntities())
        //        {
        //            cart = dbCtx.ShoppingCart.FirstOrDefault(c => c.UserId == user.Id);
        //            if (cart == null)
        //            {
        //                cart = new ShoppingCart() { UserId = user.Id };
        //                cart.AddItems(cartItems);
        //                dbCtx.ShoppingCart.Add(cart);
        //                dbCtx.SaveChanges();
        //            }
        //            else
        //            {
        //                dbCtx.CartProduct.RemoveRange(cart.CartProduct);
        //                cart.CartProduct.Clear();
        //                cart.AddItems(cartItems);
        //                dbCtx.SaveChanges();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        cart = (ShoppingCart)Session["Cart"];
        //        if (cart == null)
        //        {
        //            cart = new ShoppingCart();
        //            cart.CartProduct.Clear();
        //            cart.AddItems(cartItems);
        //            Session.Add("Cart", cart);
        //        }
        //        else
        //        {
        //            cart.CartProduct.Clear();
        //            cart.AddItems(cartItems);
        //        }
        //    }

        //    var model = new CartViewModel(cart);

        //    return View("Index", model);
        //}

        //private ShoppingCart GetCart()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    { }
        //    else
        //    {

        //    }
        //}
    }
}
