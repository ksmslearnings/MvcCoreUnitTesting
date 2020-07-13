using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitTestingAspNetCoreMVCApplication.Models;
using UnitTestingAspNetCoreMVCApplication.ServiceLayer;
using Newtonsoft.Json;

namespace UnitTestingAspNetCoreMVCApplication.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        /// <summary>
        /// Constructor - ProductService Injected using .Net Core in-built Dependency Injection Model.
        /// This interface and its concrete implementation is registered in startup.cs file.
        /// </summary>
        /// <param name="productService"></param>
        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        public IActionResult Listing()
        {
            var queryString = this.HttpContext.Request.QueryString.Value;
            if (queryString == string.Empty || queryString == null)
            {
                var viewModel = this.productService.ListProducts();
                return View("Listing", viewModel);
            }
            else
            {
                //if querystring passed- get product description and then use it for gettign filtered products.
                //if querystring is numeric then throw error.
                int checkValue = -1;
                queryString = queryString.Substring(1, queryString.Length - 1);
                string productDescription = queryString.Split('=')[1].Trim();

                if (int.TryParse(productDescription, out checkValue))
                {
                    throw new InvalidOperationException("QueryString passed does not " +
                        "have valid product description");
                }
                //check if filtered products available in session. If yes get from there.
                if (this.HttpContext.Session.GetObjectBack<List<ProductViewModel>>("ViewModel") != null)
                {
                    return View(this.HttpContext.Session
                        .GetObjectBack<List<ProductViewModel>>("ViewModel"));
                }
                else
                {
                    //get filtered product list and store in session.
                    var viewModel = this.productService.ListProducts()
                        .Where(x => x.ProductDescription.ToLower().Contains(productDescription.ToLower())).ToList();
                    this.HttpContext.Session
                        .SetObjectToString<List<ProductViewModel>>("ViewModel", viewModel);
                    return View("Listing", viewModel);
                }
            }
        }

        /// <summary>
        /// Get the Cart of the customer. we have a dummy customer for demonstration.
        /// </summary>
        /// <returns></returns>
        private BasketViewModel GetBasket()
        {
            var b = new BasketViewModel();
            b.Basket = this.productService.GetBasket("dummyuser");
            return b;
        }

        /// <summary>
        /// show basket in checkout view.
        /// </summary>
        /// <returns></returns>
        public IActionResult Checkout()
        {
            var b = GetBasket();

            return View(b);
        }


        /// <summary>
        /// check if product available for input product ID
        /// add product to cart which is static dictionary object storing list of products
        /// reduce product availability count by 1.
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public IActionResult AddToCart(int ProductID)
        {
            try
            {
                int returnValue = -1;
                ProductViewModel product = null;

                product = this.productService.GetProduct(ProductID);
                if (product != null)
                    returnValue = this.productService.AddToCartForUser(ProductID);
                if (returnValue > 0)
                    returnValue = this.productService.ReduceProductCount(ProductID);

                return View(product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// session helper extension methods to set and get the session objects.
    /// .Net core Session doesnt provide direct method to store any object and 
    /// first we need to serealize the object as string and then only can store it 
    /// in session.
    /// </summary>
    public static class SessionExtensions
    {
        public static void SetObjectToString<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectBack<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}