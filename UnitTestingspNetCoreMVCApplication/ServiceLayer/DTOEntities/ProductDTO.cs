﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTestingAspNetCoreMVCApplication.ServiceLayer.DTOEntities
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductCategory { get; set; }
        public string ProductDescription { get; set; }
        public int Available { get;  set; }
    }
}
