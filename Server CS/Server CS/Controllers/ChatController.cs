using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Server_CS;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Messeger_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        public static List<Message> Messages = new List<Message>();

        // GET: api/<ChatController>
        [HttpGet]
        public List<Message> Get()
        {
            return Messages;
        }

        // POST api/<ChatController>
        [HttpPost]
        public string Post([FromBody] Message value)
        {
            value.MsgTime = DateTime.Now;
            Console.WriteLine(value);
            Messages.Add(value);
            return "ok";
        }
    }
}