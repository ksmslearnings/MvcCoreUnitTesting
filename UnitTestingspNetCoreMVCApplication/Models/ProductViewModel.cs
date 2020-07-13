using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTestingAspNetCoreMVCApplication.Models
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductCategory { get; set; }
        public string ProductDescription { get; set; }
        public int Available { get;  set; }

        public string AvailablityStatus
        {
            get
            {
                if (this.Available == 0)
                    return "Out of Stock";
                else
                    return
                        $"Available Items: {this.Available.ToString()}";
            }
        }

      
    }
}
