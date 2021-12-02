﻿using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class TranslationsControllerTests
    {
        private readonly Mock<ITranslationsRepository> _translationsRepository = new Mock<ITranslationsRepository>();
        private readonly Random _random = new Random();

        private Translation GetRandomTranslation()
        {
            return new Translation()
            {
                Id = (uint)_random.Next(0, int.MaxValue),
                Language = new Language()
                {
                    Id = (uint)_random.Next(0, int.MaxValue),
                    Code = Guid.NewGuid().ToString(),
                    ImagePath = Guid.NewGuid().ToString()
                },
                Word = new Word()
                {
                    Id = (uint)_random.Next(0, int.MaxValue),
                    ImagePath = Guid.NewGuid().ToString()
                },
                Value = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Should always return every translation with given language
        /// </summary>
        [Fact]
        public async Task GetTranslationsAsyncTest()
        {
            // Arrange
            var expectedTranslations = new Translation[] { GetRandomTranslation(), GetRandomTranslation() };
            _translationsRepository.Setup(x => x.GetTranslationsAsync(It.IsAny<uint>())).ReturnsAsync(expectedTranslations);

            var controller = new TranslationsController(_translationsRepository.Object);

            // Act
            var actualTranslations = await controller.GetTranslationsAsync(uint.MinValue);

            // Assert
            Assert.Equal(expectedTranslations, actualTranslations);
        }

        /// <summary>
        /// Should return NotFound when translation does not exist
        /// </summary>
        [Fact]
        public async Task GetTranslationAsyncTest()
        {
            // Arrange
            _translationsRepository.Setup(x => x.GetTranslationAsync(It.IsAny<uint>())).ReturnsAsync((Translation) null);

            var controller = new TranslationsController(_translationsRepository.Object);

            // Act
            var result = await controller.GetTranslationAsync(uint.MinValue);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Should return translation when the translation exists
        /// </summary>
        [Fact]
        public async Task GetTranslationAsyncTest2()
        {
            // Arrange
            Translation expectedTranslation = GetRandomTranslation();
            _translationsRepository.Setup(x => x.GetTranslationAsync(It.IsAny<uint>())).ReturnsAsync(expectedTranslation);

            var controller = new TranslationsController(_translationsRepository.Object);

            // Act
            var result = await controller.GetTranslationAsync(uint.MinValue);

            // Assert
            Assert.Equal(expectedTranslation, result.Value);
        }

        /// <summary>
        /// Should return BadRequest when translation with given word and language already exists
        /// </summary>
        [Fact]
        public async Task CreateTranslationAsyncTest()
        {
            // Arrange
            Translation expectedTranslation = GetRandomTranslation();
            _translationsRepository.Setup(x => x.GetTranslationByWordAndLanguageAsync(It.IsAny<uint>(), It.IsAny<uint>()))
                .ReturnsAsync(expectedTranslation);

            var controller = new TranslationsController(_translationsRepository.Object);

            // Act
            var result = await controller.CreateTranslationAsync(expectedTranslation);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        /// <summary>
        /// Should return CreatedAtAction with created translation when given word and language are unique
        /// </summary>
        [Fact]
        public async Task CreateTranslationAsyncTest2()
        {
            // Arrange
            Translation expectedTranslation = GetRandomTranslation();
            _translationsRepository.Setup(x => x.GetTranslationByWordAndLanguageAsync(It.IsAny<uint>(), It.IsAny<uint>()))
                .ReturnsAsync((Translation) null);

            var controller = new TranslationsController(_translationsRepository.Object);

            // Act
            var result = await controller.CreateTranslationAsync(expectedTranslation);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

            Translation actualTranslation = (result.Result as CreatedAtActionResult).Value as Translation;

            Assert.NotNull(actualTranslation);
            Assert.Equal(expectedTranslation.Language, actualTranslation.Language);
            Assert.Equal(expectedTranslation.Word, actualTranslation.Word);
            Assert.Equal(expectedTranslation.Value, actualTranslation.Value);
        }

        /// <summary>
        /// Should always return NoContent
        /// </summary>
        [Fact]
        public async Task UpdateTranslationAsyncTest()
        {
            // Arrange
            Translation expectedTranslation = GetRandomTranslation();
            var controller = new TranslationsController(_translationsRepository.Object);

            // Act
            var result = await controller.UpdateTranslationAsync(expectedTranslation);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Should always return NoContent
        /// </summary>
        [Fact]
        public async Task DeleteTranslationAsyncTest()
        {
            // Arrange
            var controller = new TranslationsController(_translationsRepository.Object);

            // Act
            var result = await controller.DeleteTranslationAsync(uint.MinValue);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
