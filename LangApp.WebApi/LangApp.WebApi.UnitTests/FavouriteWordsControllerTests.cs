using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WebApi.UnitTests
{
    public class FavouriteWordsControllerTests
    {
        private readonly Mock<IFavouriteWordsRepository> _favouriteWordsRepository = new Mock<IFavouriteWordsRepository>();
        private readonly Random _random = new Random();

        private FavouriteWord GetRandomFavouriteWord()
        {
            return new FavouriteWord()
            {
                Id = (uint)_random.Next(0, int.MaxValue),
                User = new User()
                {
                    Id = (uint)_random.Next(0, int.MaxValue),
                    Email = Guid.NewGuid().ToString(),
                    Username = Guid.NewGuid().ToString(),
                    Password = Guid.NewGuid().ToString(),
                    Role = (UserRole) _random.Next(Enum.GetNames(typeof(UserRole)).Length - 1)
                },
                Word = new Word()
                {
                    Id = (uint)_random.Next(0, int.MaxValue),
                    ImagePath = Guid.NewGuid().ToString()
                },
                FirstLanguage = new Language()
                {
                    Id = (uint)_random.Next(0, int.MaxValue),
                    Code = Guid.NewGuid().ToString(),
                    Name = Guid.NewGuid().ToString(),
                    ImagePath = Guid.NewGuid().ToString()
                },
                SecondLanguage = new Language()
                {
                    Id = (uint)_random.Next(0, int.MaxValue),
                    Code = Guid.NewGuid().ToString(),
                    Name = Guid.NewGuid().ToString(),
                    ImagePath = Guid.NewGuid().ToString()
                }
            };
        }

        /// <summary>
        /// Should always return all favourite words
        /// </summary>
        [Fact]
        public async Task GetFavouriteWordsAsyncTest()
        {
            // Arrange
            var expectedFavouriteWords = new FavouriteWord[] { GetRandomFavouriteWord(), GetRandomFavouriteWord() };
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordsAsync()).ReturnsAsync(expectedFavouriteWords);

            var controller = new FavouriteWordsController(_favouriteWordsRepository.Object);

            // Act
            var actualFavouriteWords = await controller.GetFavouriteWordsAsync();

            // Assert
            Assert.Equal(expectedFavouriteWords, actualFavouriteWords);
        }

        /// <summary>
        /// Should always return all favourite words of user
        /// </summary>
        [Fact]
        public async Task GetFavouriteWordsOfUserAsyncTest()
        {
            // Arrange
            var expectedFavouriteWords = new FavouriteWord[] { GetRandomFavouriteWord(), GetRandomFavouriteWord() };
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordsOfUserAsync(It.IsAny<uint>())).ReturnsAsync(expectedFavouriteWords);

            var controller = new FavouriteWordsController(_favouriteWordsRepository.Object);

            // Act
            var actualFavouriteWords = await controller.GetFavouriteWordsOfUserAsync(uint.MinValue);

            // Assert
            Assert.Equal(expectedFavouriteWords, actualFavouriteWords);
        }

        /// <summary>
        /// Should return NotFound when favourite word does not exist
        /// </summary>
        [Fact]
        public async Task GetFavouriteWordAsyncTest()
        {
            // Arrange
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordAsync(It.IsAny<uint>())).ReturnsAsync((FavouriteWord) null);

            var controller = new FavouriteWordsController(_favouriteWordsRepository.Object);

            // Act
            var result = await controller.GetFavouriteWordAsync(uint.MinValue);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Should return favourite word when the favourite word exists
        /// </summary>
        [Fact]
        public async Task GetFavouriteWordAsyncTest2()
        {
            // Arrange
            FavouriteWord expectedFavouriteWord = GetRandomFavouriteWord();
            _favouriteWordsRepository.Setup(x => x.GetFavouriteWordAsync(It.IsAny<uint>())).ReturnsAsync(expectedFavouriteWord);

            var controller = new FavouriteWordsController(_favouriteWordsRepository.Object);

            // Act
            var result = await controller.GetFavouriteWordAsync(uint.MinValue);

            // Assert
            Assert.Equal(expectedFavouriteWord, result.Value);
        }

        /// <summary>
        /// Should always return CreatedAtAction with created favourite word
        /// </summary>
        [Fact]
        public async Task CreateFavouriteWordAsyncTest()
        {
            // Arrange
            FavouriteWord expectedFavouriteWord = GetRandomFavouriteWord();
            var controller = new FavouriteWordsController(_favouriteWordsRepository.Object);

            // Act
            var result = await controller.CreateFavouriteWordAsync(expectedFavouriteWord);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

            var actualFavouriteWord = (result.Result as CreatedAtActionResult).Value as FavouriteWord;

            Assert.NotNull(actualFavouriteWord);
            Assert.Equal(expectedFavouriteWord.User, actualFavouriteWord.User);
            Assert.Equal(expectedFavouriteWord.Word, actualFavouriteWord.Word);
            Assert.Equal(expectedFavouriteWord.FirstLanguage, actualFavouriteWord.FirstLanguage);
            Assert.Equal(expectedFavouriteWord.SecondLanguage, actualFavouriteWord.SecondLanguage);
        }

        /// <summary>
        /// Should always return NoContent
        /// </summary>
        [Fact]
        public async Task DeleteFavouriteWordAsyncTest()
        {
            // Arrange
            var controller = new FavouriteWordsController(_favouriteWordsRepository.Object);

            // Act
            var result = await controller.DeleteFavouriteWordAsync(uint.MinValue);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
