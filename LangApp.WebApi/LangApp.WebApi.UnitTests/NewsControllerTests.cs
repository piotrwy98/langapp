using LangApp.Shared.Models;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class NewsControllerTests
    {
        private readonly Mock<INewsRepository> _newsRepository;
        private readonly NewsController _newsController;

        public NewsControllerTests()
        {
            _newsRepository = new Mock<INewsRepository>();
            _newsController = new NewsController(_newsRepository.Object);
        }

        /// <summary>
        /// Zawsze zwraca wszystkie aktualności
        /// </summary>
        [Fact]
        public async Task GetAllNewsAsyncTest()
        {
            // Arrange
            var expectedNews = new News[] { new News(), new News() };
            _newsRepository.Setup(x => x.GetNewsAsync()).ReturnsAsync(expectedNews);

            // Act
            var actualNews = await _newsController.GetNewsAsync();

            // Assert
            Assert.Equal(expectedNews, actualNews);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje aktualność o podanym id
        /// </summary>
        [Fact]
        public async Task GetNewsAsyncTest()
        {
            // Arrange
            _newsRepository.Setup(x => x.GetNewsAsync(It.IsAny<uint>())).ReturnsAsync((News)null);

            // Act
            var result = await _newsController.GetNewsAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądaną aktualność, kiedy istnieje aktualność o podanym id
        /// </summary>
        [Fact]
        public async Task GetNewsAsyncTest2()
        {
            // Arrange
            var expectedNews = new News();
            _newsRepository.Setup(x => x.GetNewsAsync(It.IsAny<uint>())).ReturnsAsync(expectedNews);

            // Act
            var result = await _newsController.GetNewsAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedNews, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca utworzoną aktualność
        /// </summary>
        [Fact]
        public async Task CreateNewsAsyncTest()
        {
            // Arrange
            var expectedNews = new News();
            _newsRepository.Setup(x => x.CreateNewsAsync(It.IsAny<News>())).ReturnsAsync(expectedNews);

            // Act
            var result = await _newsController.CreateNewsAsync(expectedNews);

            // Assert
            Assert.Equal(expectedNews, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdateNewsAsyncTest()
        {
            // Arrange

            // Act
            var result = await _newsController.UpdateNewsAsync(It.IsAny<News>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeleteNewsAsyncTest()
        {
            // Arrange

            // Act
            var result = await _newsController.DeleteNewsAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
