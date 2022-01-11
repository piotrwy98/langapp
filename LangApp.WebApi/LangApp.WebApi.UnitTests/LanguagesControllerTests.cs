using LangApp.Shared.Models;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class LanguagesControllerTests
    {
        private readonly Mock<ILanguagesRepository> _languagesRepository;
        private readonly LanguagesController _languagesController;

        public LanguagesControllerTests()
        {
            _languagesRepository = new Mock<ILanguagesRepository>();
            _languagesController = new LanguagesController(_languagesRepository.Object);
        }

        /// <summary>
        /// Zawsze zwraca wszystkie języki
        /// </summary>
        [Fact]
        public async Task GetLanguagesAsyncTest()
        {
            // Arrange
            var expectedLanguages = new LanguageName[] { new LanguageName(), new LanguageName() };
            _languagesRepository.Setup(x => x.GetLanguagesAsync()).ReturnsAsync(expectedLanguages);

            // Act
            var actualLanguages = await _languagesController.GetLanguagesAsync();

            // Assert
            Assert.Equal(expectedLanguages, actualLanguages);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje język o podanym id
        /// </summary>
        [Fact]
        public async Task GetLanguageAsyncTest()
        {
            // Arrange
            _languagesRepository.Setup(x => x.GetLanguageAsync(It.IsAny<uint>())).ReturnsAsync((LanguageName) null);

            // Act
            var result = await _languagesController.GetLanguageAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądany język, kiedy istnieje język o podanym id
        /// </summary>
        [Fact]
        public async Task GetLanguageAsyncTest2()
        {
            // Arrange
            var expectedLanguage = new LanguageName();
            _languagesRepository.Setup(x => x.GetLanguageAsync(It.IsAny<uint>())).ReturnsAsync(expectedLanguage);

            // Act
            var result = await _languagesController.GetLanguageAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedLanguage, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca utworzony język
        /// </summary>
        [Fact]
        public async Task CreateLanguageAsyncTest()
        {
            // Arrange
            var expectedLanguage = new LanguageName();
            _languagesRepository.Setup(x => x.CreateLanguageAsync(It.IsAny<LanguageName>())).ReturnsAsync(expectedLanguage);

            // Act
            var result = await _languagesController.CreateLanguageAsync(expectedLanguage);

            // Assert
            Assert.Equal(expectedLanguage, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdateLanguageAsyncTest()
        {
            // Arrange

            // Act
            var result = await _languagesController.UpdateLanguageAsync(It.IsAny<LanguageName>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeleteLanguageAsyncTest()
        {
            // Arrange

            // Act
            var result = await _languagesController.DeleteLanguageAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
