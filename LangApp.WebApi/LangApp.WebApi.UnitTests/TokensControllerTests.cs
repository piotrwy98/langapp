using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LangApp.WebApi.UnitTests
{
    public class TokensControllerTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IUsersRepository> _usersRepository;
        private readonly TokensController _tokensController;

        public TokensControllerTests()
        {
            _configuration = new Mock<IConfiguration>();
            _usersRepository = new Mock<IUsersRepository>();
            _tokensController = new TokensController(_configuration.Object, _usersRepository.Object);
        }

        /// <summary>
        /// Zwraca Unauthorized kiedy nie istnieje u¿ytkownik o podanym adresie email
        /// </summary>
        [Fact]
        public async Task CreateTokenAsyncTest()
        {
            // Arrange
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);

            // Act
            var result = await _tokensController.CreateUserWithTokenAsync(new LogInData());

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca Unauthorized kiedy podane has³o nie jest prawid³owe
        /// </summary>
        [Fact]
        public async Task CreateTokenAsyncTest2()
        {
            // Arrange
            var firstPassword = BCrypt.Net.BCrypt.HashPassword("first_password");
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User() { Password = firstPassword });

            // Act
            var result = await _tokensController.CreateUserWithTokenAsync(new LogInData() { Password = "second_password" });

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        /// <summary>
        /// Zwraca UserWithToken kiedy podane dane s¹ prawid³owe
        /// </summary>
        [Fact]
        public async Task CreateTokenAsyncTest3()
        {
            // Arrange
            var password = "password";
            var expectedUser = new User() { Password = BCrypt.Net.BCrypt.HashPassword(password) };
            _configuration.Setup(x => x["JWTSettings:SecretKey"]).Returns("just_a_sample_jwtsettings_secretkey");
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(expectedUser);

            // Act
            var result = await _tokensController.CreateUserWithTokenAsync(new LogInData() { Password = password });

            // Assert
            Assert.Equal(expectedUser, result.Value.User);
            Assert.NotNull(result.Value.Token);
        }
    }
}
