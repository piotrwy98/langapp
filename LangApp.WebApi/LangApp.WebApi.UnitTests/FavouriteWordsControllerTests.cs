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
    public class FavouriteWordsControllerTests
    {
        private const uint USER_ID = 1;

        private readonly Mock<IFavouriteWordsRepository> _favouriteWordsRepository;
        private readonly FavouriteWordsController _favouriteWordsController;

        public FavouriteWordsControllerTests()
        {
            _favouriteWordsRepository = new Mock<IFavouriteWordsRepository>();
            _favouriteWordsController = new FavouriteWordsController(_favouriteWordsRepository.Object);

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, USER_ID.ToString()));

            _favouriteWordsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user.Object }
            };
        }

        /// <summary>
        /// Zawsze zwraca wszystkie ulubione słowa należące do użytkownika
        /// </summary>
        [Fact]
        public async Task GetFavouriteWordsAsyncTest()
        {
            // Arrange
            var expectedFavouriteWords = new FavouriteWord[] { new FavouriteWord(), new FavouriteWord() };
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordsAsync(It.IsAny<uint>())).ReturnsAsync(expectedFavouriteWords);

            // Act
            var actualFavouriteWords = await _favouriteWordsController.GetFavouriteWordsAsync();

            // Assert
            Assert.Equal(expectedFavouriteWords, actualFavouriteWords);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje ulubione słowo o podanym id
        /// </summary>
        [Fact]
        public async Task GetFavouriteWordAsyncTest()
        {
            // Arrange
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordAsync(It.IsAny<uint>())).ReturnsAsync((FavouriteWord) null);

            // Act
            var result = await _favouriteWordsController.GetFavouriteWordAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem żądanego ulubionego słowa
        /// </summary>
        [Fact]
        public async Task GetFavouriteWordAsyncTest2()
        {
            // Arrange
            var favouriteWord = new FavouriteWord() { UserId = USER_ID + 1 };
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordAsync(It.IsAny<uint>())).ReturnsAsync(favouriteWord);

            // Act
            var result = await _favouriteWordsController.GetFavouriteWordAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca żądane ulubione słowo, kiedy istnieje ulubione słowo o podanym id
        /// oraz autor zapytania jest jego właścicielem
        /// </summary>
        [Fact]
        public async Task GetFavouriteWordAsyncTest3()
        {
            // Arrange
            var expectedFavouriteWord = new FavouriteWord() { UserId = USER_ID };
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordAsync(It.IsAny<uint>())).ReturnsAsync(expectedFavouriteWord);

            // Act
            var result = await _favouriteWordsController.GetFavouriteWordAsync(It.IsAny<uint>());

            // Assert
            Assert.Equal(expectedFavouriteWord, result.Value);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem podanego ulubionego słowa
        /// </summary>
        [Fact]
        public async Task CreateFavouriteWordAsyncTest()
        {
            // Arrange
            var favouriteWord = new FavouriteWord() { UserId = USER_ID + 1 };

            // Act
            var result = await _favouriteWordsController.CreateFavouriteWordAsync(favouriteWord);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca utworzone ulubione słowo, kiedy autor zapytania jest właścicielem podanego ulubionego słowa
        /// </summary>
        [Fact]
        public async Task CreateFavouriteWordAsyncTest2()
        {
            // Arrange
            var expectedFavouriteWord = new FavouriteWord() { UserId = USER_ID };
            _favouriteWordsRepository.Setup(x => x.CreateFavouriteWordAsync(It.IsAny<FavouriteWord>())).ReturnsAsync(expectedFavouriteWord);

            // Act
            var result = await _favouriteWordsController.CreateFavouriteWordAsync(expectedFavouriteWord);

            // Assert
            Assert.Equal(expectedFavouriteWord, result.Value);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem podanego ulubionego słowa
        /// </summary>
        [Fact]
        public async Task UpdateFavouriteWordAsyncTest()
        {
            // Arrange
            var favouriteWord = new FavouriteWord() { UserId = USER_ID + 1 };

            // Act
            var result = await _favouriteWordsController.UpdateFavouriteWordAsync(favouriteWord);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        /// <summary>
        /// Zwraca NoContent, kiedy autor zapytania jest właścicielem podanego ulubionego słowa
        /// </summary>
        [Fact]
        public async Task UpdateFavouriteWordAsyncTest2()
        {
            // Arrange
            var favouriteWord = new FavouriteWord() { UserId = USER_ID };

            // Act
            var result = await _favouriteWordsController.UpdateFavouriteWordAsync(favouriteWord);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zwraca NotFound, kiedy nie istnieje ulubione słowo o podanym id
        /// </summary>
        [Fact]
        public async Task DeleteFavouriteWordAsyncTest()
        {
            // Arrange
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordAsync(It.IsAny<uint>())).ReturnsAsync((FavouriteWord) null);

            // Act
            var result = await _favouriteWordsController.DeleteFavouriteWordAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Zwraca Unauthorized, kiedy autor zapytania nie jest właścicielem żądanego ulubionego słowa
        /// </summary>
        [Fact]
        public async Task DeleteFavouriteWordAsyncTest2()
        {
            // Arrange
            var favouriteWord = new FavouriteWord() { UserId = USER_ID + 1 };
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordAsync(It.IsAny<uint>())).ReturnsAsync(favouriteWord);

            // Act
            var result = await _favouriteWordsController.DeleteFavouriteWordAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        /// <summary>
        /// Zwraca NoContent, kiedy istnieje ulubione słowo o podanym id
        /// oraz autor zapytania jest jego właścicielem
        /// </summary>
        [Fact]
        public async Task DeleteFavouriteWordAsyncTest3()
        {
            // Arrange
            var expectedFavouriteWord = new FavouriteWord() { UserId = USER_ID };
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordAsync(It.IsAny<uint>())).ReturnsAsync(expectedFavouriteWord);

            // Act
            var result = await _favouriteWordsController.DeleteFavouriteWordAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
