using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class CategoriesControllerTest
    {
        private readonly Mock<ICategoriesRepository> _categoriesRepository = new Mock<ICategoriesRepository>();

        private Category GetRandomCategory()
        {
            return new Category()
            {
                Id = Guid.NewGuid(),
                Level = new Level()
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString()
                },
                Name = Guid.NewGuid().ToString(),
                ImagePath = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Should always return all categories
        /// </summary>
        [Fact]
        public async Task GetCategoriesAsyncTest()
        {
            // Arrange
            var expectedCategories = new Category[] { GetRandomCategory(), GetRandomCategory() };
            _categoriesRepository.Setup(x => x.GetCategoriesAsync()).ReturnsAsync(expectedCategories);

            var controller = new CategoriesController(_categoriesRepository.Object);

            // Act
            var actualCategories = await controller.GetCategoriesAsync();

            // Assert
            Assert.Equal(expectedCategories, actualCategories);
        }

        /// <summary>
        /// Should return NotFound when category does not exist
        /// </summary>
        [Fact]
        public async Task GetCategoryAsyncTest()
        {
            // Arrange
            _categoriesRepository.Setup(x => x.GetCategoryAsync(It.IsAny<Guid>())).ReturnsAsync((Category) null);

            var controller = new CategoriesController(_categoriesRepository.Object);

            // Act
            var result = await controller.GetCategoryAsync(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Should return category when the category exists
        /// </summary>
        [Fact]
        public async Task GetCategoryAsyncTest2()
        {
            // Arrange
            Category expectedCategory = GetRandomCategory();
            _categoriesRepository.Setup(x => x.GetCategoryAsync(It.IsAny<Guid>())).ReturnsAsync(expectedCategory);

            var controller = new CategoriesController(_categoriesRepository.Object);

            // Act
            var result = await controller.GetCategoryAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(expectedCategory, result.Value);
        }

        /// <summary>
        /// Should always return CreatedAtAction with created category
        /// </summary>
        [Fact]
        public async Task CreateCategoryAsyncTest2()
        {
            // Arrange
            Category expectedCategory = GetRandomCategory();
            var controller = new CategoriesController(_categoriesRepository.Object);

            // Act
            var result = await controller.CreateCategoryAsync(new CategoryData()
            {
                Level = expectedCategory.Level,
                Name = expectedCategory.Name,
                ImagePath = expectedCategory.ImagePath
            });

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

            Category actualCategory = (result.Result as CreatedAtActionResult).Value as Category;

            Assert.NotNull(actualCategory);
            Assert.Equal(expectedCategory.Level, actualCategory.Level);
            Assert.Equal(expectedCategory.Name, actualCategory.Name);
            Assert.Equal(expectedCategory.ImagePath, actualCategory.ImagePath);
        }

        /// <summary>
        /// Should always return NoContent
        /// </summary>
        [Fact]
        public async Task UpdateCategoryAsyncTest()
        {
            // Arrange
            Category expectedCategory = GetRandomCategory();
            var controller = new CategoriesController(_categoriesRepository.Object);

            // Act
            var result = await controller.UpdateCategoryAsync(expectedCategory);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Should always return NoContent
        /// </summary>
        [Fact]
        public async Task DeleteCategoryAsyncTest()
        {
            // Arrange
            var controller = new CategoriesController(_categoriesRepository.Object);

            // Act
            var result = await controller.DeleteCategoryAsync(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
