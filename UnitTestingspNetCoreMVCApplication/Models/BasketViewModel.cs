using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTestingAspNetCoreMVCApplication.Models
{
    public class BasketViewModel
    {
        public List<ProductViewModel> Basket
        {
            get;
            set;
        }

    }
}
