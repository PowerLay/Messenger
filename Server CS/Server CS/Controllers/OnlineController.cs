using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Server_CS.Program;

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
            return OnlineUsers;
        }

        // POST api/<OnlineController>
        [Authorize]
        [HttpPost]
        public void Post()
        {
            var name = User.Identity.Name;
            if (name == null) return;
            if (OnlineUsers.ContainsKey(name))
            {
                OnlineUsersTimeout[name] = DateTime.Now;
            }
            else
            {
                OnlineUsers.Add(name, true);
                OnlineUsersTimeout.Add(name, DateTime.Now);

                Messages.Add(new Message
                {
                    Name = "",
                    Text = $"{name} connected",
                    Ts = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
                });

            }
        }
    }
}