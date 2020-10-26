using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Server_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] RegData login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user == null) return response;

            var tokenString = GenerateJSONWebToken(user);
            response = Ok(new { token = tokenString });

            return response;
        }

        private string GenerateJSONWebToken(RegData userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userInfo.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userInfo.Password)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RegData AuthenticateUser(RegData login)
        {
            RegData user = null;

            if (Program.RegDatas.Find(regData => regData.Username == login.Username)==default)
            {
                Program.RegDatas.Add(login);
                return login;
            }

            foreach (var regData in Program.RegDatas.Where(regData => regData.Username == login.Username && regData.Password == login.Password))
            {
                return regData;
            }

            return user;
        }
    }
}
