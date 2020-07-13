using System.Collections.Generic;
using UnitTestingAspNetCoreMVCApplication.Models;
using UnitTestingAspNetCoreMVCApplication.ServiceLayer.DTOEntities;

namespace UnitTestingAspNetCoreMVCApplication.ServiceLayer
{
    public interface IProductService
    {
        List<ProductViewModel> ListProducts();
        int AddToCartForUser(int productId);
        int ReduceProductCount(int productId);

        ProductViewModel GetProduct(int productId);

        List<ProductViewModel> GetBasket(string user);

    }
}