using LangApp.Shared.Models;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoriesRepository> _categoriesRepository;
        private readonly CategoriesController _categoriesController;

        public CategoriesControllerTests()
        {
            _categoriesRepository = new Mock<ICategoriesRepository>();
            _categoriesController = new CategoriesController(_categoriesRepository.Object);
        }

        /// <summary>
        /// Zawsze zwraca wszystkie kategorie
        /// </summary>
        [Fact]
        public async Task GetCategoriesAsyncTest()
        {
            // Arrange
            var expectedCategories = new CategoryName[] { new CategoryName(), new CategoryName() };
            _categoriesRepository.Setup(x => x.GetCategoriesAsync()).ReturnsAsync(expectedCategories);

            // Act
            var actualCategories = await _categoriesController.GetCategoriesAsync();

            // Assert
            Assert.Equal(expectedCategories, actualCategories);
        }

        /// <summary>
        /// Zwraca NotFound kiedy nie istnieje kategoria o podanym id
        /// </summary>
        [Fact]
        public async Task GetCategoryAsyncTest()
        {
            // Arrange
            _categoriesRepository.Setup(x => x.GetCategoryAsync(It.IsAny<uint>())).ReturnsAsync((CategoryName) null);

            // Act
            var result = await _categoriesController.GetCategoryAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądaną kategorię kiedy istnieje kategoria o podanym id
        /// </summary>
        [Fact]
        public async Task GetCategoryAsyncTest2()
        {
            // Arrange
            var expectedCategory = new CategoryName();
            _categoriesRepository.Setup(x => x.GetCategoryAsync(It.IsAny<uint>())).ReturnsAsync(expectedCategory);

            // Act
            var result = await _categoriesController.GetCategoryAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedCategory, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca utworzoną kategorię
        /// </summary>
        [Fact]
        public async Task CreateCategoryAsyncTest()
        {
            // Arrange
            var expectedCategory = new CategoryName();
            _categoriesRepository.Setup(x => x.CreateCategoryAsync(It.IsAny<CategoryName>())).ReturnsAsync(expectedCategory);

            // Act
            var result = await _categoriesController.CreateCategoryAsync(expectedCategory);

            // Assert
            Assert.Equal(expectedCategory, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdateCategoryAsyncTest()
        {
            // Arrange

            // Act
            var result = await _categoriesController.UpdateCategoryAsync(It.IsAny<CategoryName>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeleteCategoryAsyncTest()
        {
            // Arrange

            // Act
            var result = await _categoriesController.DeleteCategoryAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
