using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("tokens")]
    public class TokensController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IConfiguration _configuration;

        public TokensController(IConfiguration configuration, IUsersRepository usersRepository)
        {
            _configuration = configuration;
            _usersRepository = usersRepository;
        }

        [HttpPost]
        public async Task<ActionResult<UserWithToken>> CreateUserWithTokenAsync([FromBody] LogInData data)
        {
            var user = await _usersRepository.GetUserByEmailAsync(data.Email);
            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(data.Password, user.Password))
                {
                    string token = await GenerateToken(user);
                    user.Password = data.Password;

                    var userWithToken = new UserWithToken()
                    {
                        User = user,
                        Token = token,
                    };

                    return userWithToken;
                }
            }

            return Unauthorized();
        }

        private Task<string> GenerateToken(User user)
        {
            var claims = new Dictionary<string, object>();
            claims.Add(ClaimTypes.Role, user.Role.ToString());
            claims.Add(ClaimTypes.NameIdentifier, user.Id.ToString());

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWTSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Claims = claims
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }
    }
}
