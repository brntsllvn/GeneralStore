﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevLunch.Controllers;
using DevLunch.Data;
using DevLunch.Data.Models;
using NUnit.Framework;
using Shouldly;

namespace DevLunch.Tests.Controllers
{
    [TestFixture]
    public class RestaurantControllerTests
    {
        [Test]
        public void Details_ReturnsOneRestaurant()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            context.Restaurants.Add(new Restaurant { Name = "Brave Horse" });
            context.SaveChanges();
            var controller = new RestaurantController(context);

            // Act
            var Id = context.Restaurants.First().Id;
            var result = controller.Details(Id) as ViewResult;

            // Assert
            var data = result.Model as Restaurant;
            data.ShouldNotBeNull();
            data.Id.ShouldBe(1);
            data.Name.ShouldBe("Brave Horse");
        }

        [Test]
        public void Details_WithoutIdThrows()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act
            var result = controller.Details(null) as HttpStatusCodeResult;

            // Assert
            result.ShouldBeOfType<HttpStatusCodeResult>();
            result.StatusCode.ShouldBe(400);
        }

        [Test]
        public void Detail_ReturnsNullException_WhenRecordDoesNotExist()
        {
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act 
            var result = controller.Details(999) as HttpStatusCodeResult;

            // Assert
            result.ShouldBeOfType<HttpNotFoundResult>();
            result.StatusCode.ShouldBe(404);
        }

        [Test]
        public void Index_ReturnsAllRestaurants()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            context.Restaurants.Add(new Restaurant { Name = "Brave Horse" });
            context.Restaurants.Add(new Restaurant { Name = "Yard House" });
            context.SaveChanges();

            var controller = new RestaurantController(context);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            var data = result.Model as IEnumerable<Restaurant>;
            data.ShouldNotBeNull();
            data.Count().ShouldBe(2);
            data.First().Name.ShouldBe("Brave Horse");
            data.Last().Name.ShouldBe("Yard House");
        }

        [Test]
        public void Create_Get_CreatesDefaultAndShowsItInTheView()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act
            var result = controller.Create();

            // Assert
            result.ShouldNotBeNull();
            result.Model.ShouldBeOfType<Restaurant>();
        }

        [Test]
        public void Create_Post_CreatesNewRestaurantAndSavesToDb()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act
            var result = controller.Create(new Restaurant {Name = "Brent's Pub"}) as RedirectToRouteResult;

            // Assert
            context.Restaurants.First(r=>r.Name == "Brent's Pub").ShouldNotBeNull();
            result.RouteValues["action"].ShouldBe("Index");
        }

        [Test]
        public void Create_Post_DoesNotSaveToDBAndReturnsCreateViewIfNameIsTooShort()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act
            var restaurant = new Restaurant { Name = "B" };
            var result = controller.Create(restaurant) as ViewResult;

            // Assert
            result.Model.ShouldBe(restaurant);
        }

        [Test]
        public void Edit_Get_ShowsRestaurantInTheView()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            context.Restaurants.Add(new Restaurant { Name = "Brave Horse" });
            context.SaveChanges();
            var controller = new RestaurantController(context);

            // Act
            var Id = context.Restaurants.First().Id;
            var result = controller.Edit(Id) as ViewResult;

            // Assert
            result.ShouldNotBeNull();
            result.Model.ShouldBeOfType<Restaurant>();
        }

        [Test]
        public void Edit_Get_ThrowsIfRestaurantIdIsNull()
        {
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act
            var result = controller.Edit(null) as HttpStatusCodeResult;

            // Assert
            result.ShouldBeOfType<HttpStatusCodeResult>();
            result.StatusCode.ShouldBe(400);
        }

        [Test]
        public void Edit_Get_ThrowsNotFoundIfRestaurantNotInDb()
        {
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act 
            var result = controller.Details(999) as HttpStatusCodeResult;

            // Assert
            result.ShouldBeOfType<HttpNotFoundResult>();
            result.StatusCode.ShouldBe(404);
        }

        [Test]
        public void Edit_Post_EditsRestaurantAndSavesToDb()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            context.Restaurants.Add(new Restaurant { Name = "Brave Horse" });
            context.SaveChanges();
            var controller = new RestaurantController(context);

            var editableRestaurantId = context.Restaurants.First().Id;
            var restaurantEditGetResult = controller.Edit(editableRestaurantId) as ViewResult;

            var restaurantToEdit = restaurantEditGetResult.Model as Restaurant;
            restaurantToEdit.Name = "Linda's";
            restaurantToEdit.Longitude = -5;
            restaurantToEdit.Latitude = 5;

            // Act
            var result = controller.Edit(editableRestaurantId, restaurantToEdit) as RedirectToRouteResult;

            // Assert
            context.Restaurants.First().Name.ShouldBe("Linda's");
            result.RouteValues["action"].ShouldBe("Index");
        }

        [Test]
        public void Delete_RemovesRestaurantFromDatabase()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            context.Restaurants.Add(new Restaurant { Name = "Brave Horse" });
            context.SaveChanges();
            var controller = new RestaurantController(context);

            // Act
            var Id = context.Restaurants.First().Id;
            var result = controller.Delete(Id) as RedirectToRouteResult;

            // Assert
            context.Restaurants.ShouldBeEmpty();
            result.RouteValues["action"].ShouldBe("Index");
        }

        [Test]
        public void Delete_ThrowsWhenIdIsNull()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act
            var result = controller.Delete(null) as HttpStatusCodeResult;

            // Assert
            result.ShouldBeOfType<HttpStatusCodeResult>();
            result.StatusCode.ShouldBe(400);
        }

        [Test]
        public void Delete_ThrowsWhenRestaurantCannotBeFound()
        {
            // Arrange
            var context = new DevLunchDbContext(Effort.DbConnectionFactory.CreateTransient());
            var controller = new RestaurantController(context);

            // Act
            var result = controller.Delete(999) as HttpStatusCodeResult;

            // Assert
            result.ShouldBeOfType<HttpNotFoundResult>();
            result.StatusCode.ShouldBe(404);
        }
    }
}
