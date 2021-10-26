using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
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

        /// <summary>
        /// Should return Unauthorized when email does not exist
        /// </summary>
        [Fact]
        public async Task CreateTokenAsyncTest()
        {
            // Arrange
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);
            var controller = new TokensController(_configuration.Object, _usersRepository.Object);

            // Act
            var result = await controller.CreateUserWithTokenAsync(new LogInData() { Email = String.Empty, Password = String.Empty });

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Should return Unauthorized when password is not valid
        /// </summary>
        [Fact]
        public async Task CreateTokenAsyncTest2()
        {
            // Arrange
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
            var controller = new TokensController(_configuration.Object, _usersRepository.Object);

            // Act
            var result = await controller.CreateUserWithTokenAsync(new LogInData() { Email = String.Empty, Password = String.Empty });

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Should return UserWithToken when user authentication passed
        /// </summary>
        [Fact]
        public async Task CreateTokenAsyncTest3()
        {
            // Arrange
            User expectedUser = new User() { Password = "password" };
            _configuration.Setup(x => x["JWTSettings:SecretKey"]).Returns("just_a_sample_jwtsettings_secretkey");
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(expectedUser);
            var controller = new TokensController(_configuration.Object, _usersRepository.Object);

            // Act
            var result = await controller.CreateUserWithTokenAsync(new LogInData() { Email = String.Empty, Password = expectedUser.Password });

            // Assert
            Assert.IsType<UserWithToken>(result.Value);
            Assert.NotNull(result.Value.Token);
            Assert.Equal(expectedUser, result.Value.User);
        }
    }
}
