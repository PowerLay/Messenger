using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace Client_CS_CLI
{
    internal class Program
    {
        /// <summary>
        ///     Точка входа
        /// </summary>
        private static void Main()
        {
            ConfigManager.LoadConfig();

            Login();

            var onlineUpdaterThread = new Thread(ServerResponse.OnlineUpdater) {Name = "OnlineUpdaterThread"};
            onlineUpdaterThread.Start();

            var historyUpdaterThread = new Thread(ServerResponse.GetHistoryMessages) {Name = "HistoryUpdaterThread"};
            historyUpdaterThread.Start();

            while (true)
                try
                {
                    while (true) Post();
                }
                catch (Exception)
                {
                    // ignored
                }
        }

        /// <summary>
        ///     Запрос у пользователя уникального ника/пароля
        /// </summary>
        private static void Login()
        {
            do
            {
                var httpWebRequest = (HttpWebRequest) WebRequest.Create("http://localhost:5000/api/Login");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string result;
                var regData = GetRegData(httpWebRequest);

                try
                {
                    var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                    var streamReader =
                        new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException());
                    result = streamReader.ReadToEnd();
                }
                catch (Exception)
                {
                    Console.SetCursorPosition(0, 0);
                    continue;
                }

                var temp = JsonConvert.DeserializeAnonymousType(result, new {Token = ""});

                ConfigManager.Config.Token = temp.Token;

                ConfigManager.Config.RegData = regData;
                Console.WriteLine("Success!");
                ConfigManager.WriteConfig();
                break;
            } while (true);
        }

        /// <summary>
        ///     Запрос на уникальность ника
        /// </summary>
        /// <param name="httpWebRequest">Веб запрос</param>
        /// <returns>Связка Логин пароль</returns>
        private static RegData GetRegData(HttpWebRequest httpWebRequest)
        {
            Console.Write("Enter your nick> ".PadRight(Console.BufferWidth - 1));
            Console.Write("".PadRight(Console.BufferWidth - 1));
            Console.SetCursorPosition(0, 0);
            Console.Write("Enter your nick> ");
            var nick = Console.ReadLine();
            Console.Write("Enter your password> ");
            var password = Console.ReadLine();

            var regData = new RegData {Username = nick, Password = password};
            var json = JsonConvert.SerializeObject(regData);
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();

            return regData;
        }

        /// <summary>
        ///     <para>Функция ввода и печати сообщения в час (отправляется POST запрос на сервер)</para>
        /// </summary>
        private static async void Post()
        {
            Console.Write("Enter message(or /u for update)>        \b\b\b\b\b\b\b");
            var msg = Console.ReadLine();
            if (msg.Equals("/update") || msg.Equals("/u"))
            {
                await ServerResponse.UpdateHistory();
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                return;
            }

            var httpWebRequest = (HttpWebRequest) WebRequest.Create("http://localhost:5000/api/Chat");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + ConfigManager.Config.Token);

            SendMessage(msg, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        /// <summary>
        ///     Функция получения ответа от сервер после отправки сообщения
        /// </summary>
        /// <param name="httpWebRequest">Веб запрос</param>
        private static void GetAnswer(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            using var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            if (result != "ok") Console.WriteLine("Something went wrong");
        }

        /// <summary>
        ///     Функция отправки сообщения на сервер
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="httpWebRequest">Веб запрос</param>
        private static void SendMessage(string msg, HttpWebRequest httpWebRequest)
        {
            var json = JsonConvert.SerializeObject(new Message {Text = msg});
            using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }
}