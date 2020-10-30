using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client_CS_CLI
{
    internal class Program : ConfigManager
    {
        private static int _len;

        private static void Main(string[] args)
        {
            LoadConfig();

            do
            {
                Console.Write("Enter your nick> ".PadRight(Console.BufferWidth - 1));
                Console.Write("".PadRight(Console.BufferWidth - 1));
                Console.SetCursorPosition(0, 0);
                Console.Write("Enter your nick> ");
                var nick = Console.ReadLine();
                Console.Write("Enter your password> ");
                var password = Console.ReadLine();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/api/Login");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                var regData = new RegData() { Username = nick, Password = password };
                var json = JsonConvert.SerializeObject(regData);
                var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                streamWriter.Write(json);
                streamWriter.Close();

                string result = "";

                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    var streamReader = new StreamReader(httpResponse.GetResponseStream());
                    result = streamReader.ReadToEnd();
                }
                catch (Exception)
                {
                    Console.SetCursorPosition(0, 0);
                    continue;
                }

                var temp = JsonConvert.DeserializeObject<TokenResponse>(result);


                ConfigManager.Config.Token = temp.Token;

                ConfigManager.Config.RegData = regData;
                Console.WriteLine("Success!");
                ConfigManager.WriteConfig();
                break;
            } while (true);


            Thread onlineUpdaterThread = new Thread(OnlineUpdater);
            onlineUpdaterThread.Start();

            while (true)
                try
                {
                    GetHistoryMessages();
                    while (true) Post();
                }
                catch (Exception)
                {
                    // ignored
                }
        }

        private static async Task PostOnline()
        {
            try
            {
                var httpWebRequest =
                    (HttpWebRequest)WebRequest.Create("http://localhost:5000/api/Online");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + ConfigManager.Config.Token);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = await streamReader.ReadToEndAsync();
                if (result != "ok") throw new Exception("Something went wrong");
            }
            catch (Exception)
            {

            }
        }

        public static async void OnlineUpdater()
        {
            while (true)
            {
                try
                {
                    await PostOnline();
                    Thread.Sleep(500);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        class TokenResponse
        {
            public string Token { get; set; } = "";
        }

        public static async Task<string> GetAsync(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static async void GetHistoryMessages()
        {
            while (true)
            {
                await UpdateHistory();

                Thread.Sleep(Config.MillisecondsSleep);
            }
        }

        private static async Task UpdateHistory()
        {
            var res = await GetAsync("http://localhost:5000/api/Chat");

            if (res != "[]")
            {
                var messages = JsonConvert.DeserializeObject<List<Message>>(res);

                if (_len != messages.Count)
                {
                    var x = Console.CursorLeft;
                    var y = Console.CursorTop;

                    Console.MoveBufferArea(0, y, x, 1, 0, messages.Count + 1);

                    var history = "";
                    foreach (var message in messages)
                        history += message.ToString().PadRight(Console.BufferWidth - 1) + "\n";
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine(history);
                    Console.SetCursorPosition(x, messages.Count + 1);

                    _len = messages.Count;
                }
            }
        }
        /// <summary>
        /// <para>Функция ввода и печати сообщения в час (отправляется POST запрос на сервер)</para>
        /// </summary>
        private static void Post()
        {
            Console.Write("Enter message(or /u for update)>        \b\b\b\b\b\b\b");
            var msg = Console.ReadLine();
            if (msg.Equals("/update") || msg.Equals("/u"))
            {
                UpdateHistory();
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                return;
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/api/Chat");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + Config.Token);

            SendMessage(msg, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        /// <summary>
        /// <para>Функция получения ответа от сервера</para>
        /// </summary>
        /// <param name="httpWebRequest"></param>
        private static void GetAnswer(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            if (result != "ok") Console.WriteLine("Something went wrong");
        }

        /// <summary>
        /// <para>Функция отправки сообщения на сервер</para>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="httpWebRequest"></param>
        private static void SendMessage(string msg, HttpWebRequest httpWebRequest)
        {
            var json = JsonConvert.SerializeObject(new Message { Text = msg });
            using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }
}