using LangApp.Shared.Models;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class TranslationsControllerTests
    {
        private readonly Mock<ITranslationsRepository> _translationsRepository;
        private readonly TranslationsController _translationsController;

        public TranslationsControllerTests()
        {
            _translationsRepository = new Mock<ITranslationsRepository>();
            _translationsController = new TranslationsController(_translationsRepository.Object);
        }

        /// <summary>
        /// Zawsze zwraca wszystkie tłumaczenia
        /// </summary>
        [Fact]
        public async Task GetTranslationsAsyncTest()
        {
            // Arrange
            var expectedTranslations = new Translation[] { new Translation(), new Translation() };
            _translationsRepository.Setup(x => x.GetTranslationsAsync()).ReturnsAsync(expectedTranslations);

            // Act
            var actualTranslations = await _translationsController.GetTranslationsAsync();

            // Assert
            Assert.Equal(expectedTranslations, actualTranslations);
        }

        /// <summary>
        /// Zwraca NotFound kiedy nie istnieje tłumaczenie o podanym id
        /// </summary>
        [Fact]
        public async Task GetTranslationAsyncTest()
        {
            // Arrange
            _translationsRepository.Setup(x => x.GetTranslationAsync(It.IsAny<uint>())).ReturnsAsync((Translation) null);

            // Act
            var result = await _translationsController.GetTranslationAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądane tłumaczenie kiedy istnieje tłumaczenie o podanym id
        /// </summary>
        [Fact]
        public async Task GetTranslationAsyncTest2()
        {
            // Arrange
            var expectedTranslation = new Translation();
            _translationsRepository.Setup(x => x.GetTranslationAsync(It.IsAny<uint>())).ReturnsAsync(expectedTranslation);

            // Act
            var result = await _translationsController.GetTranslationAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedTranslation, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca utworzone tłumaczenie
        /// </summary>
        [Fact]
        public async Task CreateTranslationAsyncTest()
        {
            // Arrange
            var expectedTranslation = new Translation();
            _translationsRepository.Setup(x => x.CreateTranslationAsync(It.IsAny<Translation>())).ReturnsAsync(expectedTranslation);

            // Act
            var result = await _translationsController.CreateTranslationAsync(expectedTranslation);

            // Assert
            Assert.Equal(expectedTranslation, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdateTranslationAsyncTest()
        {
            // Arrange

            // Act
            var result = await _translationsController.UpdateTranslationAsync(It.IsAny<Translation>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeleteTranslationAsyncTest()
        {
            // Arrange

            // Act
            var result = await _translationsController.DeleteTranslationAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
