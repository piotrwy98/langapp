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
    public class AnswersControllerTests
    {
        private const uint USER_ID = 1;

        private readonly Mock<IAnswersRepository> _answersRepository;
        private readonly Mock<ISessionsRepository> _sessionsRepository;
        private readonly AnswersController _answersController;

        public AnswersControllerTests()
        {
            _answersRepository = new Mock<IAnswersRepository>();
            _sessionsRepository = new Mock<ISessionsRepository>();
            _answersController = new AnswersController(_answersRepository.Object, _sessionsRepository.Object);

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, USER_ID.ToString()));

            _answersController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user.Object }
            };
        }

        /// <summary>
        /// Zawsze zwraca wszystkie odpowiedzi należące do użytkownika
        /// </summary>
        [Fact]
        public async Task GetAnswersAsyncTest()
        {
            // Arrange
            var expectedAnswers = new Answer[] { new Answer(), new Answer() };
            _answersRepository.Setup(x => x.GetAnswersAsync(It.IsAny<uint>())).ReturnsAsync(expectedAnswers);

            // Act
            var actualAnswers = await _answersController.GetAnswersAsync();

            // Assert
            Assert.Equal(expectedAnswers, actualAnswers);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje odpowiedź o podanym id
        /// </summary>
        [Fact]
        public async Task GetAnswerAsyncTest()
        {
            // Arrange
            _answersRepository.Setup(x => x.GetAnswerAsync(It.IsAny<uint>())).ReturnsAsync((Answer) null);

            // Act
            var result = await _answersController.GetAnswerAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje sesja powiązana z żądaną odpowiedzią
        /// </summary>
        [Fact]
        public async Task GetAnswerAsyncTest2()
        {
            // Arrange
            _answersRepository.Setup(x => x.GetAnswerAsync(It.IsAny<uint>())).ReturnsAsync(new Answer());
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync((Session) null);

            // Act
            var result = await _answersController.GetAnswerAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem sesji powiązanej z żądaną odpowiedzią
        /// </summary>
        [Fact]
        public async Task GetAnswerAsyncTest3()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID + 1 };
            _answersRepository.Setup(x => x.GetAnswerAsync(It.IsAny<uint>())).ReturnsAsync(new Answer());
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);

            // Act
            var result = await _answersController.GetAnswerAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądaną odpowiedź, kiedy istnieje odpowiedź o podanym id oraz istnieje
        /// powiązana z nią sesja, której właścicielem jest autor zapytania
        /// </summary>
        [Fact]
        public async Task GetAnswerAsyncTest4()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID };
            _answersRepository.Setup(x => x.GetAnswerAsync(It.IsAny<uint>())).ReturnsAsync(new Answer());
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);
            var expectedAnswer = new Answer();
            _answersRepository.Setup(x => x.GetAnswerAsync(It.IsAny<uint>())).ReturnsAsync(expectedAnswer);

            // Act
            var result = await _answersController.GetAnswerAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedAnswer, result.Value);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje sesja powiązana z podaną odpowiedzią
        /// </summary>
        [Fact]
        public async Task CreateAnswerAsyncTest()
        {
            // Arrange
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync((Session) null);

            // Act
            var result = await _answersController.CreateAnswerAsync(new Answer());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem sesji powiązanej z podaną odpowiedzią
        /// </summary>
        [Fact]
        public async Task CreateAnswerAsyncTest2()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID + 1 };
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);

            // Act
            var result = await _answersController.CreateAnswerAsync(new Answer());

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca utworzoną odpowiedź, kiedy istnieje powiązana z podaną
        /// odpowiedzią sesja, której właścicielem jest autor zapytania
        /// </summary>
        [Fact]
        public async Task CreateAnswerAsyncTest3()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID };
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);
            var expectedAnswer = new Answer();
            _answersRepository.Setup(x => x.CreateAnswerAsync(It.IsAny<Answer>())).ReturnsAsync(expectedAnswer);

            // Act
            var result = await _answersController.CreateAnswerAsync(new Answer());

            // Assert
            Assert.Equal(expectedAnswer, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdateAnswerAsyncTest()
        {
            // Arrange

            // Act
            var result = await _answersController.UpdateAnswerAsync(It.IsAny<Answer>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeleteAnswerAsyncTest()
        {
            // Arrange

            // Act
            var result = await _answersController.DeleteAnswerAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
