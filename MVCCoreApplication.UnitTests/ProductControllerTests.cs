using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using UnitTestingAspNetCoreMVCApplication.Controllers;
using UnitTestingAspNetCoreMVCApplication.Models;
using UnitTestingAspNetCoreMVCApplication.ServiceLayer;
using Moq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace MVCCoreApplication.UnitTests
{

    [TestClass]
    public class ProductControllerShould
    {
        #region variables
        static List<ProductViewModel> productViewModel = null;

        Mock<IProductService> productServiceMoq = null;
        ControllerContext controllerContextMock = null;
        Mock<HttpContext> httpContextMock = null;
        Mock<HttpRequest> httpRequestMock = null;
        Mock<ISession> sessionMock = null;

        #endregion variables

        #region Class Initialize and Cleanup
        [ClassInitialize]
        public static void ProductControllerRequiredObjects(TestContext context)
        {
            productViewModel = new List<ProductViewModel>
            {
                new ProductViewModel { ProductID=1, ProductCategory=1, ProductDescription="Nokia Mobile", ProductName="Lumia", Available=5 }
            };
        }
        
        [ClassCleanup]
        public static void ProductControllerCleanStaticObjects()
        {
            productViewModel = null;
        }

        #endregion Class Initialize and Cleanup

        #region Test Initialize and Cleanup

    

        [TestInitialize]
        public void ProductControllerMockObjectsSetup()
        {
            productServiceMoq = new Mock<IProductService>();
            productServiceMoq.Setup(x => x.ListProducts()).Returns(productViewModel);

            controllerContextMock = new ControllerContext();
            httpContextMock = new Mock<HttpContext>();//its an abstract class in .net core.
            httpRequestMock = new Mock<HttpRequest>();//its an abstract class in .net core.
            sessionMock = new Mock<ISession>();

            sessionMock.Setup(x => x.Set("testKey", new byte[] { 123 }));
            //sessionMock.Setup(x => x.GetObjectBack<List<ProductViewModel>>(It.IsAny<string>())).Returns(productViewModel);

            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(x => x.Session).Returns(sessionMock.Object);
            controllerContextMock.HttpContext = httpContextMock.Object;

            //// Below line doesnt work since HttpContext is not overridable property.
            //controllerContextMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        }

        [TestCleanup]
        public void CleanAllMockedObjects()
        {
            productServiceMoq = null;
            controllerContextMock = null;
            httpContextMock = null;
            httpRequestMock = null;
            sessionMock = null;
        }

        #endregion Test Initialize and Cleanup

        #region Product Controller Test Methods

        [TestMethod]
        public void ListAllProductsWhenNoQueryStringPassed()
        {
            //Arrange
            //sut == System Under Test
            ProductController sut = new ProductController(productServiceMoq.Object);
            sut.ControllerContext = controllerContextMock;
            sut.ControllerContext.HttpContext = httpContextMock.Object;

            httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString());

            //Act
            var returnValue = sut.Listing();


            //Assert
            returnValue.Should().BeViewResult("We return Product Listing view back with Products List", null).WithViewName("Listing");
            returnValue.Should().BeViewResult().ModelAs<List<ProductViewModel>>();


        }


        [TestMethod]
        public void ListFilteredProductsWhenQueryStringPassed()
        {
            //Arrange            
            httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?p=nokia"));
            //sut == System Under Test
            ProductController sut = new ProductController(productServiceMoq.Object);
            sut.ControllerContext = controllerContextMock;
            sut.ControllerContext.HttpContext = httpContextMock.Object;

            //Act
            var returnValue = sut.Listing();

            //Assert
            returnValue.Should().BeViewResult("We return selective Product back based on product name querystring filter", null).WithViewName("Listing");
            returnValue.Should().BeViewResult().ModelAs<List<ProductViewModel>>();
        }

        [TestMethod]
        public void ThrowErrorWhenQueryStringIsNotValidProductDescription()
        {
            //Arrange            
            httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?p=123"));
            //sut == System Under Test
            ProductController sut = new ProductController(productServiceMoq.Object);
            sut.ControllerContext = controllerContextMock;
            sut.ControllerContext.HttpContext = httpContextMock.Object;

            //Act
            Func<IActionResult> act = () => sut.Listing();
            //var returnValue = sut.Listing();

            //Assert
            act.Should().Throw<InvalidOperationException>("Query string does not contain valid product description")
                .WithMessage("QueryString passed does not have valid product description")
                .And.Should().NotBeAssignableTo(typeof(IActionResult));


        }

        #endregion Product Controller Test Methods
    }
}
