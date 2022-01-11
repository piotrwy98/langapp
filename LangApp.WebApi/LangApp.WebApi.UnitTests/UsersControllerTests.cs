using LangApp.Shared.Models;
using LangApp.WebApi.Api.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WebApi.UnitTests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUsersRepository> _usersRepository;
        private readonly UsersController _usersController;

        public UsersControllerTests()
        {
            _usersRepository = new Mock<IUsersRepository>();
            _usersController = new UsersController(_usersRepository.Object);
        }

        /// <summary>
        /// Zawsze zwraca wszystkich u¿ytkowników z has³ami i emailami ustawionymi na null
        /// </summary>
        [Fact]
        public async Task GetUsersAsyncTest()
        {
            // Arrange
            var expectedUsers = new User[]
            {
                new User() { Password = "first_password", Email = "first_email" },
                new User() { Password = "second_password", Email = "second_email" }
            };
            _usersRepository.Setup(x => x.GetUsersAsync()).ReturnsAsync(expectedUsers);

            // Act
            var actualUsers = await _usersController.GetUsersAsync();
            var areNull = true;

            foreach (var user in actualUsers)
            {
                if (user.Password != null || user.Email != null)
                {
                    areNull = false;
                    break;
                }
            }

            // Assert
            Assert.Equal(expectedUsers, actualUsers);
            Assert.True(areNull);
        }

        /// <summary>
        /// Zwraca NotFound kiedy nie istnieje u¿ytkownik o podanym id
        /// </summary>
        [Fact]
        public async Task GetUserAsyncTest()
        {
            // Arrange
            _usersRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<uint>())).ReturnsAsync((User) null);

            // Act
            var result = await _usersController.GetUserAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Zwraca ¿¹danego u¿ytkownika z has³em i emailem ustawionymi na null kiedy istnieje u¿ytkownik o podanym id
        /// </summary>
        [Fact]
        public async Task GetUserAsyncTest2()
        {
            // Arrange
            var expectedUser = new User() { Password = "password", Email = "email" };
            _usersRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<uint>())).ReturnsAsync(expectedUser);

            // Act
            var result = await _usersController.GetUserAsync(It.IsAny<uint>());
            var areNull = result.Value.Password == null && result.Value.Email == null;

            // Assert
            Assert.Equal(expectedUser, result.Value);
            Assert.True(areNull);
        }

        /// <summary>
        /// Zwraca BadRequestObject z RegisterResult.OCCUPIED_EMAIL kiedy istnieje u¿ytkownik o podanym emailu
        /// </summary>
        [Fact]
        public async Task CreateUserAsyncTest()
        {
            // Arrange
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

            // Act
            var result = await _usersController.CreateUserAsync(new User());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(RegisterResult.OCCUPIED_EMAIL, (result.Result as BadRequestObjectResult).Value);
        }

        /// <summary>
        /// Zwraca BadRequestObject z RegisterResult.OCCUPIED_USERNAME kiedy istnieje u¿ytkownik o podanej nazwie
        /// </summary>
        [Fact]
        public async Task CreateUserAsyncTest2()
        {
            // Arrange
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);
            _usersRepository.Setup(x => x.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            // Act
            var result = await _usersController.CreateUserAsync(new User());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(RegisterResult.OCCUPIED_USERNAME, (result.Result as BadRequestObjectResult).Value);
        }

        /// <summary>
        /// Zwraca utworzonego u¿ytkownika z has³em o zmienionej postaci oraz rol¹ UserRole.USER
        /// kiedy nie istnieje u¿ytkownik o podanej nazwie lub emailu
        /// </summary>
        [Fact]
        public async Task CreateUserAsyncTest3()
        {
            // Arrange
            var password = "password";
            var expectedUser = new User() { Password = password, Email = "email", Role = UserRole.ADMIN };
            _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((User) null);
            _usersRepository.Setup(x => x.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User) null);
            _usersRepository.Setup(x => x.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(expectedUser);

            // Act
            var result = await _usersController.CreateUserAsync(expectedUser);

            // Assert
            Assert.Equal(expectedUser, result.Value);
            Assert.NotEqual(result.Value.Password, password);
            Assert.Equal(UserRole.USER, result.Value.Role);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task UpdateUserAsyncTest()
        {
            // Arrange

            // Act
            var result = await _usersController.UpdateUserAsync(It.IsAny<User>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Zawsze zwraca NoContent
        /// </summary>
        [Fact]
        public async Task DeleteUserAsyncTest()
        {
            // Arrange

            // Act
            var result = await _usersController.DeleteUserAsync(It.IsAny<uint>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
