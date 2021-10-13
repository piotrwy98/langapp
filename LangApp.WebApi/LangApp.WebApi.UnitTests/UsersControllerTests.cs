using LangApp.Shared.Models;
using LangApp.WebApi.Controllers;
using LangApp.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WebApi.UnitTests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUsersRepository> _usersRepository = new Mock<IUsersRepository>();
        private readonly Random _random = new Random();

        private User GetRandomUser()
        {
            return new User()
            {
                Id = Guid.NewGuid(),
                Email = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                UserRole = (UserRole)_random.Next(Enum.GetNames(typeof(UserRole)).Length - 1)
            };
        }

        /// <summary>
        /// Should always return all existing users
        /// </summary>
        [Fact]
        public async Task GetUsersAsyncTest()
        {
            // Arrange
            var expectedUsers = new User[] { GetRandomUser(), GetRandomUser() };
            _usersRepository.Setup(x => x.GetUsersAsync()).ReturnsAsync(expectedUsers);

            var controller = new UsersController(_usersRepository.Object);

            // Act
            var actualUsers = await controller.GetUsersAsync();

            // Assert
            Assert.Equal(expectedUsers, actualUsers);
        }

        /// <summary>
        /// Should return NotFound when user does not exist
        /// </summary>
        [Fact]
        public async Task GetUserAsyncTest()
        {
            // Arrange
            _usersRepository.Setup(x => x.GetUserAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var controller = new UsersController(_usersRepository.Object);

            // Act
            var result = await controller.GetUserAsync(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Should return user when user exists
        /// </summary>
        [Fact]
        public async Task GetUserAsyncTest2()
        {
            // Arrange
            User expectedUser = GetRandomUser();
            _usersRepository.Setup(x => x.GetUserAsync(It.IsAny<Guid>())).ReturnsAsync(expectedUser);

            var controller = new UsersController(_usersRepository.Object);

            // Act
            var result = await controller.GetUserAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(expectedUser, result.Value);
        }

        /// <summary>
        /// Should always return CreatedAtAction with created user
        /// </summary>
        [Fact]
        public async Task CreateUserAsyncTest()
        {
            // Arrange
            User expectedUser = GetRandomUser();
            var controller = new UsersController(_usersRepository.Object);

            // Act
            var result = await controller.CreateUserAsync(expectedUser.Email, expectedUser.Username, expectedUser.Password, expectedUser.UserRole);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

            User actualUser = (result.Result as CreatedAtActionResult).Value as User;

            Assert.NotNull(actualUser);
            Assert.Equal(expectedUser.Email, actualUser.Email);
            Assert.Equal(expectedUser.Username, actualUser.Username);
            Assert.Equal(expectedUser.Password, actualUser.Password);
            Assert.Equal(expectedUser.UserRole, actualUser.UserRole);
        }

        /// <summary>
        /// Should always return NoContent
        /// </summary>
        [Fact]
        public async Task UpdateUserAsyncTest()
        {
            // Arrange
            User expectedUser = GetRandomUser();
            var controller = new UsersController(_usersRepository.Object);

            // Act
            var result = await controller.UpdateUserAsync(expectedUser);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Should always return NoContent
        /// </summary>
        [Fact]
        public async Task DeleteUserAsyncTest()
        {
            // Arrange
            var controller = new UsersController(_usersRepository.Object);

            // Act
            var result = await controller.DeleteUserAsync(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
