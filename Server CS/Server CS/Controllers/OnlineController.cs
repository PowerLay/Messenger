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
        // GET: api/<OnlineController>
        [HttpGet]
        public Dictionary<string, bool> Get()
        {
            return Program.OnlineUsers;
        }

        // POST api/<OnlineController>
        [Authorize]
        [HttpPost]
        public string Post()
        {
            var name = User.Identity.Name;
            if (name == null) return "No name";
            if (Program.OnlineUsers.ContainsKey(name))
            {
                if (!Program.OnlineUsers[name])
                {
                    Program.Messages.Add(new Message
                    {
                        Name = "",
                        Text = $"{name} connected",
                        Ts = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
                    });
                    JsonWorker.Save(Program.Messages);
                    Program.OnlineUsers[name] = true;
                }

                Program.OnlineUsersTimeout[name] = DateTime.Now;
            }
            else
            {
                Program.OnlineUsers.Add(name, true);
                Program.OnlineUsersTimeout.Add(name, DateTime.Now);

                Program.Messages.Add(new Message
                {
                    Name = "",
                    Text = $"{name} connected",
                    Ts = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
                });
                JsonWorker.Save(Program.Messages);
            }

            return "ok";
        }
    }
}