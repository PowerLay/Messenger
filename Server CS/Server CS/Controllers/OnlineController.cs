using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Server_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineController : ControllerBase
    {
        /// <summary>
        ///     Запрос на получение всех пользователей и их состояния (В сети/Не в сети)
        ///     <br>GET: api/Online</br>
        /// </summary>
        /// <returns>Словарь пользователей (В сети/Не в сети)</returns>
        [HttpGet]
        public List<string> Get()
        {
            return Program.OnlineUsers;
        }

        /// <summary>
        ///     Получения сигнала онлайн пользователя
        ///     <br>POST: api/Online</br>
        /// </summary>
        /// <returns>Возвращает ok если всё прошло успешно</returns>
        [Authorize]
        [HttpPost]
        public string Post()
        {
            var name = User.Identity.Name;
            if (name == null) return "No name";
            if (Program.OnlineUsers.Contains(name))
            {
                Program.OnlineUsersTimeout[name] = DateTime.Now;
            }
            else
            {
                Program.OnlineUsers.Add(name);
                Program.OnlineUsersTimeout.Add(name, DateTime.Now);

                Program.Messages.Add(new Message
                {
                    Name = "",
                    Text = $"{name} connected",
                    Ts = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
                });
            }

            return "ok";
        }
    }
}