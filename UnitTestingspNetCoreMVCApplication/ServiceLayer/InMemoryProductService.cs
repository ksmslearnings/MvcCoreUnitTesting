using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTestingAspNetCoreMVCApplication.Models;
using UnitTestingAspNetCoreMVCApplication.ServiceLayer.DTOEntities;

namespace UnitTestingAspNetCoreMVCApplication.ServiceLayer
{
    public class InMemoryProductService : IProductService
    {
        // public static readonly List<ProductDTO> products = null;
        //private static InMemoryProductService()
        //{
        static List<ProductDTO> products = null;
        static Dictionary<string, List<ProductDTO>> cart = new Dictionary<string, List<ProductDTO>>();

        //    }

        static InMemoryProductService()
        {

            products = new List<ProductDTO>
            {
                new ProductDTO { ProductID=1, ProductCategory=1, ProductDescription="Nokia Mobile", ProductName="Lumia", Available=5},
                new ProductDTO {ProductID=2, ProductCategory=2, ProductDescription="Samsung TV", ProductName="X10", Available=5 },
                new ProductDTO { ProductID=3, ProductCategory=1, ProductDescription="Apple iPhone", ProductName="iPhone X", Available=5},
                new ProductDTO { ProductID=4, ProductCategory=3, ProductDescription="XBox Gaming", ProductName="XBox 360", Available=5}
            };

        }
        
        List<ProductViewModel> IProductService.ListProducts()
        {
            try
            {
                var viewModel = products.Select(x => new ProductViewModel
                {
                    ProductID = x.ProductID,
                    ProductName = x.ProductName,
                    ProductCategory = x.ProductCategory,
                    ProductDescription = x.ProductDescription,
                    Available = x.Available
                }
                 );

                return viewModel.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;

        }

        int IProductService.AddToCartForUser(int productId)
        {
            List<ProductDTO> addedProducts = new List<ProductDTO>();
            var p = products.Where(x => x.ProductID == productId && x.Available >= 1).FirstOrDefault();

            if (p != null)
            {
                if (!cart.ContainsKey("dummyuser"))
                {
                    addedProducts.Add(p);
                    cart.Add("dummyuser", addedProducts);
                }
                else
                {
                    cart["dummyuser"].Add(p);
                }
                return 1;
            }
            else
            {
                return 0;
            }
        }

        ProductViewModel IProductService.GetProduct(int productId)
        {
            var p = products.Where(x => x.ProductID == productId).FirstOrDefault();
            if (p != null)
                return new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductCategory = p.ProductCategory,
                    Available = p.Available
                };
            else
                return null;

        }

        int IProductService.ReduceProductCount(int productId)
        {
            var p = products.Where(x => x.ProductID == productId && x.Available >= 1).FirstOrDefault();
            if (p != null)
            {
                p.Available = p.Available - 1;
                return 1;
            }
            else
            {
                return 0;
            }
        }


        List<ProductViewModel> IProductService.GetBasket(string user)
        {
            List<ProductViewModel> vm = new List<ProductViewModel>();

            var items = cart[user];
            vm = items.Select(x => new ProductViewModel
            {
                ProductID = x.ProductID,
                ProductName = x.ProductName,
                ProductDescription = x.ProductDescription,
                ProductCategory = x.ProductCategory,
                

            }).ToList();

            return vm;
        }
    }
}
