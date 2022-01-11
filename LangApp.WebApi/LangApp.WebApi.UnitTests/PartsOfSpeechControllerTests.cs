using LangApp.Shared.Models;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class PartsOfSpeechControllerTests
    {
        private readonly Mock<IPartsOfSpeechRepository> _partsOfSpeechRepository;
        private readonly PartsOfSpeechController _partsOfSpeechController;

        public PartsOfSpeechControllerTests()
        {
            _partsOfSpeechRepository = new Mock<IPartsOfSpeechRepository>();
            _partsOfSpeechController = new PartsOfSpeechController(_partsOfSpeechRepository.Object);
        }

        /// <summary>
        /// Zawsze zwraca wszystkie części mowy
        /// </summary>
        [Fact]
        public async Task GetPartsOfSpeechAsyncTest()
        {
            // Arrange
            var expectedPartsOfSpeech = new PartOfSpeechName[] { new PartOfSpeechName(), new PartOfSpeechName() };
            _partsOfSpeechRepository.Setup(x => x.GetPartsOfSpeechAsync()).ReturnsAsync(expectedPartsOfSpeech);

            // Act
            var actualPartsOfSpeech = await _partsOfSpeechController.GetPartsOfSpeechAsync();

            // Assert
            Assert.Equal(expectedPartsOfSpeech, actualPartsOfSpeech);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje część mowy o podanym id
        /// </summary>
        [Fact]
        public async Task GetPartOfSpeechAsyncTest()
        {
            // Arrange
            _partsOfSpeechRepository.Setup(x => x.GetPartOfSpeechAsync(It.IsAny<uint>())).ReturnsAsync((PartOfSpeechName) null);

            // Act
            var result = await _partsOfSpeechController.GetPartOfSpeechAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądaną część mowy, kiedy istnieje część mowy o podanym id
        /// </summary>
        [Fact]
        public async Task GetPartOfSpeechAsyncTest2()
        {
            // Arrange
            var expectedPartOfSpeech = new PartOfSpeechName();
            _partsOfSpeechRepository.Setup(x => x.GetPartOfSpeechAsync(It.IsAny<uint>())).ReturnsAsync(expectedPartOfSpeech);

            // Act
            var result = await _partsOfSpeechController.GetPartOfSpeechAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedPartOfSpeech, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca utworzoną część mowy
        /// </summary>
        [Fact]
        public async Task CreatePartOfSpeechAsyncTest()
        {
            // Arrange
            var expectedPartOfSpeech = new PartOfSpeechName();
            _partsOfSpeechRepository.Setup(x => x.CreatePartOfSpeechAsync(It.IsAny<PartOfSpeechName>())).ReturnsAsync(expectedPartOfSpeech);

            // Act
            var result = await _partsOfSpeechController.CreatePartOfSpeechAsync(expectedPartOfSpeech);

            // Assert
            Assert.Equal(expectedPartOfSpeech, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdatePartOfSpeechAsyncTest()
        {
            // Arrange

            // Act
            var result = await _partsOfSpeechController.UpdatePartOfSpeechAsync(It.IsAny<PartOfSpeechName>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeletePartOfSpeechAsyncTest()
        {
            // Arrange

            // Act
            var result = await _partsOfSpeechController.DeletePartOfSpeechAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
