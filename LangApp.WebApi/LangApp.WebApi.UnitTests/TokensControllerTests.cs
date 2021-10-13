using LangApp.WebApi.Controllers;
using LangApp.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class TokensControllerTests
    {
        private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
        private readonly Mock<IUsersRepository> _usersRepository = new Mock<IUsersRepository>();
        private readonly Random _random = new Random();

        /// <summary>
        /// Should return Unauthorized when user authentication failed
        /// </summary>
        [Fact]
        public async Task CreateTokenAsyncTest()
        {
            // Arrange
            _configuration.Setup(x => x["JWTSettings:SecretKey"]).Returns("just_a_sample_jwtsettings_secretkey");
            _usersRepository.Setup(x => x.DoesUserExistAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            var controller = new TokensController(_configuration.Object, _usersRepository.Object);

            // Act
            var result = await controller.CreateTokenAsync(String.Empty, String.Empty);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Should return string token when user authentication passed
        /// </summary>
        [Fact]
        public async Task CreateTokenAsyncTest2()
        {
            // Arrange
            _configuration.Setup(x => x["JWTSettings:SecretKey"]).Returns("just_a_sample_jwtsettings_secretkey");
            _usersRepository.Setup(x => x.DoesUserExistAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            var controller = new TokensController(_configuration.Object, _usersRepository.Object);

            // Act
            var result = await controller.CreateTokenAsync(String.Empty, String.Empty);

            // Assert
            Assert.IsType<string>(result.Value);
        }
    }
}
