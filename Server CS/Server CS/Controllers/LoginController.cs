using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Server_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        /// <summary>
        ///     Объект работы с файлом конфигурации приложения
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        ///     Конструктор
        /// </summary>
        /// <param name="config">Объект работы с файлом конфигурации приложения</param>
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        ///     Проверка на уникальность ника
        ///     <br> GET api/Login</br>
        /// </summary>
        /// <param name="username">Ник</param>
        /// <returns>Возвращает истину если ник занят</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get(string username)
        {
            return Ok(new {response = Program.RegDatas.Find(regData => regData.Username == username) != default});
        }

        /// <summary>
        ///     Функция Логин/Регистрация
        ///     <br> POST api/Login</br>
        /// </summary>
        /// <param name="login">Связка логин/пароль</param>
        /// <returns>Вернет ok если всё успешно</returns>
        [HttpPost]
        public IActionResult Login([FromBody] RegData login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user == null) return response;
            if (string.IsNullOrEmpty(user.Username)) return response;

            var tokenString = GenerateJSONWebToken(user);
            response = Ok(new {token = tokenString});

            return response;
        }

        /// <summary>
        ///     Функция генерации токена
        /// </summary>
        /// <param name="userInfo">Связка логин/пароль</param>
        /// <returns>Токен</returns>
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

        /// <summary>
        ///     Функция авторизации пользователя.
        ///     <br>Если нету то создаст.</br>
        /// </summary>
        /// <param name="login">Связка логин/пароль</param>
        /// <returns>Пользователя чата</returns>
        private RegData AuthenticateUser(RegData login)
        {
            RegData user = null;

            if (Program.RegDatas.Find(regData => regData.Username == login.Username) == default)
            {
                Program.RegDatas.Add(login);
                return login;
            }

            foreach (var regData in Program.RegDatas.Where(regData =>
                regData.Username == login.Username && regData.Password == login.Password)) return regData;

            return user;
        }
    }
}