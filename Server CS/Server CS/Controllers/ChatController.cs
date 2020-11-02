using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        /// <summary>
        ///     Возвращает всю историю сообщений
        ///     <br> GET api/Chat</br>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Message> Get()
        {
            return Program.Messages;
        }

        /// <summary>
        ///     <para>Post function</para>
        ///     <br>Печать времени отправки сообщения</br>
        ///     <br>Добавление сообщения в глобальный массив сообщений</br>
        ///     <br>Сохранение массива сообщений в файл в виде json объекта</br>
        ///     <br> POST api/Chat</br>
        /// </summary>
        /// <param name="value">Сообщение</param>
        /// <returns>Возвращает ok если всё успешно</returns>
        [Authorize]
        [HttpPost]
        public string Post([FromBody] Message value)
        {
            value.Ts = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var msg = new Message {Name = User.Identity.Name, Text = value.Text, Ts = value.Ts};
            Console.WriteLine(msg);
            Program.Messages.Add(msg);
            return "ok";
        }
    }
}