﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // Arrange
            var mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {ProductId = 1, Name = "P1"},
                new Product() {ProductId = 2, Name = "P2"},
                new Product() {ProductId = 3, Name = "P3"},
                new Product() {ProductId = 4, Name = "P4"},
                new Product() {ProductId = 5, Name = "P5"}
            });
            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            //var result = (IEnumerable<Product>) controller.List(2).Model;
            var result = (ProductsListViewModel) controller.List(null, 2).Model;

            // Assert
            var prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // Arrange - define an HTML helper - we need to do this
            // in order to apply the extension method

            // Arrange - create PagingInfo data
            var pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            var result = PagingHelpers.PageLinks(null, pagingInfo, pageUrlDelegate);

            // Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>" + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>" + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            var mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {ProductId = 1, Name = "P1"},
                new Product() {ProductId = 2, Name = "P2"},
                new Product() {ProductId = 3, Name = "P3"},
                new Product() {ProductId = 4, Name = "P4"},
                new Product() {ProductId = 5, Name = "P5"}
            });

            // Arrange
            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            var result = (ProductsListViewModel) controller.List(null, 2).Model;

            // Assert
            var pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filtere_Products()
        {
            // Arrange
            // - create the mock repository
            var mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {ProductId = 1, Name = "P1", Category = "Cat1"},
                new Product() {ProductId = 2, Name = "P2", Category = "Cat2"},
                new Product() {ProductId = 3, Name = "P3", Category = "Cat1"},
                new Product() {ProductId = 4, Name = "P4", Category = "Cat2"},
                new Product() {ProductId = 5, Name = "P5", Category = "Cat3"}
            });
            
            // Arrange - create a controller and make the page size 3 items
            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            var result = ((ProductsListViewModel) controller.List("Cat2", 1).Model).Products.ToArray();

            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange
            // - create the mock repository
            var mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {ProductId = 1, Name = "P1", Category = "Apples"},
                new Product() {ProductId = 2, Name = "P2", Category = "Apples"},
                new Product() {ProductId = 3, Name = "P3", Category = "Plums"},
                new Product() {ProductId = 4, Name = "P4", Category = "Oranges"}
            });

            // Arrange - create the controller
            var target = new NavController(mock.Object);

            // Act = get the set of categories
            var results = ((IEnumerable<string>) target.Menu().Model).ToArray();

            // Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            // - create the mock repository
            var mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductId = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductId = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductId = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductId = 5, Name = "P5", Category = "Cat3"}
            });

            // Arrange - create a controller and make the page size 3 items
            var target = new ProductController(mock.Object) {PageSize = 3};

            // Action - test the product counts for different categories
            var res1 = ((ProductsListViewModel) target.List("Cat1").Model).PagingInfo.TotalItems;
            var res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            var res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            var resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            // Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }

        [TestMethod]
        public void Can_Add_New_Lines()
        {
            // Arrange - create some test products
            var p1 = new Product {ProductId = 1, Name = "P1"};
            var p2 = new Product {ProductId = 2, Name = "P2"};

            // Arrange - create a new cart
            var target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            var results = target.Lines.ToArray();

            // Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            // Arrange - create some test products
            var p1 = new Product {ProductId = 1, Name = "P1"};
            var p2 = new Product { ProductId = 2, Name = "P2" };

            // Arrange - create a new cart
            var target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            var results = target.Lines.OrderBy(c => c.Product.ProductId).ToArray();

            // Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            // Arrange - create some test products
            var p1 = new Product { ProductId = 1, Name = "P1"};
            var p2 = new Product { ProductId = 2, Name = "P2" };
            var p3 = new Product { ProductId = 3, Name = "P3" };

            // Arrange - create a new cart
            var target = new Cart();

            // Arrange - add some products to the cart
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            // Act
            target.RemoveLine(p2);

            // Assert
            Assert.AreEqual(target.Lines.Count(c => c.Product == p2), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            // Arrange - create some test products
            var p1 = new Product { ProductId = 1, Name = "P1", Price = 100M};
            var p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            // Arrange - create a new cart
            var target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            // Assert
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            // Arrange - create some test products
            var p1 = new Product { ProductId = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            // Arrange - create a new cart
            var target = new Cart();

            // Arrange - add some items
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            // Act - reset the cart
            target.Clear();

            // Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }
    }
}
