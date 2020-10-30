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
        public static Dictionary<string, bool> OnlineUsers = new Dictionary<string, bool>();
        public static Dictionary<string, DateTime> OnlineUsersTimeout = new Dictionary<string, DateTime>();

        public static void Main(string[] args)
        {
            JsonWorker.Load();
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
    }
}