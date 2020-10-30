using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Server_CS
{
    public class Program
    {
        /// <summary>
        ///     <para>Глобальный объект сообщения, в котором хранятся все сообщения в чате</para>
        /// </summary>
        public static List<RegData> RegDatas = new List<RegData>();
        public static List<Message> Messages = new List<Message>();
        public static Dictionary<string,bool> OnlineUsers = new Dictionary<string, bool>();
        public static Dictionary<string,DateTime> OnlineUsersTimeout = new Dictionary<string, DateTime>();

        public static void Main(string[] args)
        {
            JsonWorker.Load();
            Thread onlineCheckerThread = new Thread(OnlineChecker);
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        ///     <para>Создание веб-сервера для обработки запросов</para>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

        public static void OnlineChecker()
        {
            while (true)
            {
                foreach (var user in OnlineUsersTimeout)
                {
                    if (user.Value.AddSeconds(5) >= DateTime.Now)
                    {
                        OnlineUsers[user.Key] = false;
                        Messages.Add(new Message
                        {
                            Name = "",
                            Text = $"{user.Key} left",
                            Ts = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
                        });
                    }
                }
                Thread.Sleep(200);
            }
        }
    }
}