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
    public class SessionsControllerTests
    {
        private const uint USER_ID = 1;

        private readonly Mock<ISessionsRepository> _sessionsRepository;
        private readonly SessionsController _sessionsController;

        public SessionsControllerTests()
        {
            _sessionsRepository = new Mock<ISessionsRepository>();
            _sessionsController = new SessionsController(_sessionsRepository.Object);

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, USER_ID.ToString()));

            _sessionsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user.Object }
            };
        }

        /// <summary>
        /// Zawsze zwraca wszystkie sesje należące do użytkownika
        /// </summary>
        [Fact]
        public async Task GetSessionsAsyncTest()
        {
            // Arrange
            var expectedSessions = new Session[] { new Session(), new Session() };
            _sessionsRepository.Setup(x => x.GetSessionsAsync(It.IsAny<uint>())).ReturnsAsync(expectedSessions);

            // Act
            var actualSessions = await _sessionsController.GetSessionsAsync();

            // Assert
            Assert.Equal(expectedSessions, actualSessions);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje sesja o podanym id
        /// </summary>
        [Fact]
        public async Task GetSessionAsyncTest()
        {
            // Arrange
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync((Session)null);

            // Act
            var result = await _sessionsController.GetSessionAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem żądanej sesji
        /// </summary>
        [Fact]
        public async Task GetSessionAsyncTest2()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID + 1 };
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(session);

            // Act
            var result = await _sessionsController.GetSessionAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądaną sesję, kiedy istnieje sesja o podanym id oraz autor zapytania jest jej właścicielem
        /// </summary>
        [Fact]
        public async Task GetSessionAsyncTest3()
        {
            // Arrange
            var expectedSession = new Session() { UserId = USER_ID };
            _sessionsRepository.Setup(x => x.GetSessionAsync(It.IsAny<uint>())).ReturnsAsync(expectedSession);

            // Act
            var result = await _sessionsController.GetSessionAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedSession, result.Value);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem podanej sesji
        /// </summary>
        [Fact]
        public async Task CreateSessionAsyncTest()
        {
            // Arrange
            var session = new Session() { UserId = USER_ID + 1 };

            // Act
            var result = await _sessionsController.CreateSessionAsync(session);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca utworzoną sesję, kiedy autor zapytania jest właścicielem podanej sesji
        /// </summary>
        [Fact]
        public async Task CreateSessionAsyncTest2()
        {
            // Arrange
            var expectedSession = new Session() { UserId = USER_ID };
            _sessionsRepository.Setup(x => x.CreateSessionAsync(It.IsAny<Session>())).ReturnsAsync(expectedSession);

            // Act
            var result = await _sessionsController.CreateSessionAsync(expectedSession);

            // Assert
            Assert.Equal(expectedSession, result.Value);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdateSessionAsyncTest()
        {
            // Arrange

            // Act
            var result = await _sessionsController.UpdateSessionAsync(It.IsAny<Session>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeleteSessionAsyncTest()
        {
            // Arrange

            // Act
            var result = await _sessionsController.DeleteSessionAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
