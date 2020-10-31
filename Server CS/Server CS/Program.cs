using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Server_CS
{
    public class Program
    {
        /// <summary>
        ///     Глобальный объект в котором хранятся связки логин/пароль
        /// </summary>
        public static List<RegData> RegDatas = new List<RegData>();

        /// <summary>
        ///     Глобальный объект в котором хранится вся история сообщений
        /// </summary>
        public static List<Message> Messages = new List<Message>();

        /// <summary>
        ///     Словарь Пользователей (В сети/Не в сети)
        /// </summary>
        public static Dictionary<string, bool> OnlineUsers = new Dictionary<string, bool>();

        /// <summary>
        ///     Словарь пользователей и времени последнего сигнала онлайн
        /// </summary>
        public static Dictionary<string, DateTime> OnlineUsersTimeout = new Dictionary<string, DateTime>();

        /// <summary>
        ///     Точка входа
        /// </summary>
        /// <param name="args">Входящие аргументы</param>
        public static void Main(string[] args)
        {
            JsonWorker.Load();
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        ///     Создание веб-сервера для обработки запросов
        /// </summary>
        /// <param name="args">Входящие аргументы</param>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}