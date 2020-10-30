using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server_CS;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Messeger_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        // GET: api/<ChatController>
        [HttpGet]
        public List<Message> Get()
        {
            return Program.Messages;
        }

        // POST api/<ChatController>
        /// <summary>
        ///     <para>Post function</para>
        ///     <br>Печать времени отправки сообщения</br>
        ///     <br>Добавление сообщения в глобальный массив сообщений</br>
        ///     <br>Сохранение массива сообщений в файл в виде json объекта</br>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public string Post([FromBody] Message value)
        {
            value.Ts = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var msg = new Message {Name = User.Identity.Name, Text = value.Text, Ts = value.Ts};
            Console.WriteLine(msg);
            Program.Messages.Add(msg);
            JsonWorker.Save(Program.Messages);
            return "ok";
        }
    }
}