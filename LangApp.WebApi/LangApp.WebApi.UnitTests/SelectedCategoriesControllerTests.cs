using LangApp.Shared.Models;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class SelectedCategoriesControllerTests
    {
        private const uint USER_ID = 1;

        private readonly Mock<ISelectedCategoriesRepository> _selectedCategorysRepository;
        private readonly Mock<ISessionsRepository> _sessionsRepository;
        private readonly SelectedCategoriesController _selectedCategorysController;

        public SelectedCategoriesControllerTests()
        {
            _selectedCategorysRepository = new Mock<ISelectedCategoriesRepository>();
            _sessionsRepository = new Mock<ISessionsRepository>();
            _selectedCategorysController = new SelectedCategoriesController(_selectedCategorysRepository.Object, _sessionsRepository.Object);

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, USER_ID.ToString()));

            _selectedCategorysController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user.Object }
            };
        }

        /// <summary>
        /// Zawsze zwraca wszystkie wybrane kategorie należące do użytkownika
        /// </summary>
        [Fact]
        public async Task GetSelectedCategoriesAsyncTest()
        {
            // Arrange
            var expectedSelectedCategories = new SelectedCategory[] { new SelectedCategory(), new SelectedCategory() };
            _selectedCategorysRepository.Setup(x => x.GetSelectedCategoriesAsync(It.IsAny<uint>())).ReturnsAsync(expectedSelectedCategories);

            // Act
            var actualSelectedCategories = await _selectedCategorysController.GetSelectedCategoriesAsync();

            // Assert
            Assert.Equal(expectedSelectedCategories, actualSelectedCategories);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje wybrana kategoria o podanym id
        /// </summary>
        [Fact]
        public async Task GetSelectedCategoryAsyncTest()
        {
            // Arrange
            _selectedCategorysRepository.Setup(x => x.GetSelectedCategoryAsync(It.IsAny<uint>())).ReturnsAsync((SelectedCategory)null);

            // Act
            var result = await _selectedCategorysController.GetSelectedCategoryAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje sesja powiązana z żądaną wybraną kategorią
        /// </summary>
        [Fact]
        public async Task GetSelectedCategoryAsyncTest2()
        {
            // Arrange
            _selectedCategorysRepository.Setup(x => x.GetSelectedCategoryAsync(It.IsAny<uint>())).ReturnsAsync(new SelectedCategory());
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync((Session) null);

            // Act
            var result = await _selectedCategorysController.GetSelectedCategoryAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem sesji powiązanej z żądaną wybraną kategorią
        /// </summary>
        [Fact]
        public async Task GetSelectedCategoryAsyncTest3()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID + 1 };
            _selectedCategorysRepository.Setup(x => x.GetSelectedCategoryAsync(It.IsAny<uint>())).ReturnsAsync(new SelectedCategory());
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);

            // Act
            var result = await _selectedCategorysController.GetSelectedCategoryAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądaną wybraną kategorię, kiedy istnieje wybrana kategoria o podanym id oraz istnieje
        /// powiązana z nią sesja, której właścicielem jest autor zapytania
        /// </summary>
        [Fact]
        public async Task GetSelectedCategoryAsyncTest4()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID };
            _selectedCategorysRepository.Setup(x => x.GetSelectedCategoryAsync(It.IsAny<uint>())).ReturnsAsync(new SelectedCategory());
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);
            var expectedSelectedCategory = new SelectedCategory();
            _selectedCategorysRepository.Setup(x => x.GetSelectedCategoryAsync(It.IsAny<uint>())).ReturnsAsync(expectedSelectedCategory);

            // Act
            var result = await _selectedCategorysController.GetSelectedCategoryAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedSelectedCategory, result.Value);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje sesja powiązana z podaną wybraną kategorią
        /// </summary>
        [Fact]
        public async Task CreateSelectedCategoryAsyncTest()
        {
            // Arrange
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync((Session)null);

            // Act
            var result = await _selectedCategorysController.CreateSelectedCategoryAsync(new SelectedCategory());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem sesji powiązanej z podaną wybraną kategorią
        /// </summary>
        [Fact]
        public async Task CreateSelectedCategoryAsyncTest2()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID + 1 };
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);

            // Act
            var result = await _selectedCategorysController.CreateSelectedCategoryAsync(new SelectedCategory());

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca utworzoną wybraną kategorię, kiedy istnieje powiązana z podaną
        /// wybraną kategorią sesja, której właścicielem jest autor zapytania
        /// </summary>
        [Fact]
        public async Task CreateSelectedCategoryAsyncTest3()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID };
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);
            var expectedSelectedCategory = new SelectedCategory();
            _selectedCategorysRepository.Setup(x => x.CreateSelectedCategoryAsync(It.IsAny<SelectedCategory>())).ReturnsAsync(expectedSelectedCategory);

            // Act
            var result = await _selectedCategorysController.CreateSelectedCategoryAsync(new SelectedCategory());

            // Assert
            Assert.Equal(expectedSelectedCategory, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdateSelectedCategoryAsyncTest()
        {
            // Arrange

            // Act
            var result = await _selectedCategorysController.UpdateSelectedCategoryAsync(It.IsAny<SelectedCategory>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeleteSelectedCategoryAsyncTest()
        {
            // Arrange

            // Act
            var result = await _selectedCategorysController.DeleteSelectedCategoryAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
