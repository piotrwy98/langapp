using LangApp.Shared.Models;
using LangApp.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LangApp.WebApi.Controllers
{
    [ApiController]
    [Route("/tokens")]
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
                if(user.Password == data.Password)
                {
                    string token = await GenerateToken(data.Email);
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

        private Task<string> GenerateToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWTSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddMonths(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }
    }
}
