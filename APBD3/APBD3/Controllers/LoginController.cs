using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using APBD3.Models;
using APBD3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APBD3.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        public IStudentsDbService StudentsDb { get; set; }
        public LoginController(IConfiguration configuration, IStudentsDbService studentsDb)
        {
            Configuration = configuration;
            StudentsDb = studentsDb;
        }

        [HttpPost("")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            Console.WriteLine(loginModel.Index + " " + loginModel.Password);
            bool isLoggedIn = StudentsDb.canLogIn(loginModel);
            if (!isLoggedIn)
            {
                return Unauthorized("Wrong combination!");
            }

            var claims = new[]
            {
                new Claim("Index", loginModel.Index),
                new Claim("Password", loginModel.Password),
                new Claim(ClaimTypes.Role, "employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecrecikSecrecikSecrecikSecrecikKrecik"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                claims: claims,
                issuer: "Maciej",
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
        }

    }

}

