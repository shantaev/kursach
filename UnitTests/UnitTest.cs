using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebUI.Controllers;
using WebUI.Infrastructure.Abstract;
using WebUI.Models;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // Создание имитированного хранилища
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"}
                }.AsQueryable);

            // Создание контроллера и установка размера страницы равным трем позициям
            var controller = new ProductController(mock.Object) { PageSize = 3 };

            // Действие 
            var result = (ProductListViewModel)controller.List(null,2).Model;

            // Утверждение
            var prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"}
                }.AsQueryable);

            // Создание контроллера и установка размере страницы равным трем позициям
            var controller = new ProductController(mock.Object) { PageSize = 3 };

            // Действие
            var result = (ProductListViewModel) controller.List(null,2).Model;

            //Утверждение
            var pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage,3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        /// <summary>
        /// Фильтрация по категории. Для проверки корректности фильтрации по категории
        /// </summary>
        [TestMethod]
        public void Can_Filter_Products()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                }.AsQueryable);

            var controller = new ProductController(mock.Object);

            var result = ((ProductListViewModel) controller.List("Cat2", 1).Model).Products.ToArray();

            Assert.AreEqual(result.Length,2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        // Генерация списка категорий
        [TestMethod]
        public void Can_Create_Categories()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                    new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                    new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                    new Product {ProductID = 4, Name = "P4", Category = "Oranges"}
                }.AsQueryable);

            var target = new NavController(mock.Object);

            var results = ((IEnumerable<string>) target.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                    new Product {ProductID = 4, Name = "P4", Category = "Oranges"}
                }.AsQueryable);

            var target = new NavController(mock.Object);

            // определние выбранной категории
            const string categoryToSelect = "Apples";

            var result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);
        }

        /// <summary>
        /// Счетчик товаров определенной категории
        /// </summary>
        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                }.AsQueryable);

            var target = new ProductController(mock.Object) {PageSize = 3};

            // действие , тестирование счетчиков товаров для различных категорий
            int res1 = ((ProductListViewModel) target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel) target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel) target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductListViewModel) target.List(null).Model).PagingInfo.TotalItems;

            // утверждение
            Assert.AreEqual(res1,2);
            Assert.AreEqual(res2,2);
            Assert.AreEqual(res3,1);
            Assert.AreEqual(resAll,5);
        }

        /// <summary>
        /// Тестирование возможности добавление нового товара в корзину
        /// </summary>
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            var p1 = new Product{ ProductID = 1, Name = "P1"};
            var p2 = new Product {ProductID = 2, Name = "P2"};

            var target = new Cart();

            // Действие
            target.AddItem(p1,1);
            target.AddItem(p2,1);

            var results = target.Lines.ToArray();

            Assert.AreEqual(results.Length,2);
            Assert.AreEqual(results[0].Product,p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        /// <summary>
        /// Тест, на возможность увеличение количество товара, а не создавать новый
        /// </summary>
        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            var p1 = new Product { ProductID = 1, Name = "P1" };
            var p2 = new Product { ProductID = 2, Name = "P2" };
            
            var target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1,10);

            var results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        /// <summary>
        /// Удаление товара из корзины
        /// </summary>
        [TestMethod]
        public void Can_Remove_Line()
        {
            var p1 = new Product { ProductID = 1, Name = "P1" };
            var p2 = new Product { ProductID = 2, Name = "P2" };
            var p3 = new Product { ProductID = 3, Name = "P3" };

            var target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2,1);

            target.RemoveLine(p2);

            Assert.AreEqual(target.Lines.Count(c => c.Product == p2), 0);

        }

        /// <summary>
        /// Возможность вычисления общей стоимости товаров в корзине
        /// </summary>
        [TestMethod]
        public void Calculate_Cart_Total()
        {
            var p1 = new Product { ProductID = 1, Name = "P1", Price = 100};
            var p2 = new Product { ProductID = 2, Name = "P2", Price = 50};

            var target = new Cart();

            target.AddItem(p1,1);
            target.AddItem(p2,1);
            target.AddItem(p1,3);

            var result = target.ComputeTotalValue();

            Assert.AreEqual(result, 450);
        }

        /// <summary>
        /// Удаление товаров из корзины
        /// </summary>
        [TestMethod]
        public void Can_Glear_Contents()
        {
            var p1 = new Product { ProductID = 1, Name = "P1", Price = 100 };
            var p2 = new Product { ProductID = 2, Name = "P2", Price = 50 };

            var target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            target.Clear();

            Assert.AreEqual(target.Lines.Count(), 0);
        }

        /// <summary>
        /// тестирование корректного возврата объектов Product, находящихся в хранилище
        /// </summary>
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"}
                }.AsQueryable);

            var target = new AdminController(mock.Object);

            var result = ((IEnumerable<Product>) target.Index().ViewData.Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        /// <summary>
        /// проверка редактиование товара, получаем ли ожидаемый товар? 
        /// </summary>
        [TestMethod]
        public void Can_Edit_Product()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"}
                }.AsQueryable);

            var target = new AdminController(mock.Object);

            var p1 = target.Edit(1).ViewData.Model as Product;
            var p2 = target.Edit(2).ViewData.Model as Product;
            var p3 = target.Edit(3).ViewData.Model as Product;

            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        /// <summary>
        /// при передаче в качестве параметра идентификатора ProductID метод действие  должен вызвать DeleteProduct
        /// </summary>
        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            var prod = new Product {ProductID = 2, Name = "Test"};

            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    prod,
                    new Product {ProductID = 3, Name = "P3"}
                }.AsQueryable());

            var target = new AdminController(mock.Object);

            target.Delete(prod.ProductID);

            // проверка, что метод удаления в хранилище вызывается для корректного объекта Product
            mock.Verify(m => m.DeleteProduct(prod));
        }


        /// <summary>
        /// проверка аутентификации
        /// </summary>
        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {
            var mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);

            var model = new LogOnViewModel
                {
                    UserName = "admin",
                    Password = "secret"
                };

            var target = new AccountController(mock.Object);
            // аутентификация с использованием правильных учетных данных
            var result = target.LogOn(model, "/MyURL");

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }
    }
}
