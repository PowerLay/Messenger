using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client_CS_CLI
{
    internal class ServerResponse
    {
        /// <summary>
        ///     Предыдущая длина списка сообщений
        /// </summary>
        private static int _len;

        /// <summary>
        ///     Отправка сигнала о том что пользователь онлайн
        /// </summary>
        private static async Task PostOnline()
        {
            try
            {
                var httpWebRequest =
                    (HttpWebRequest) WebRequest.Create($"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Online");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + ConfigManager.Config.Token);
                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = await streamReader.ReadToEndAsync();
                if (result != "ok") throw new Exception("Something went wrong");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        ///     Цикл отправки сигнала что пользователь онлайн
        /// </summary>
        public static async void OnlineUpdater()
        {
            while (true)
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

        /// <summary>
        ///     Получает ответ на GET запрос
        /// </summary>
        /// <param name="uri">Ссылка на HTTP</param>
        /// <returns>Ответ на GET запрос</returns>
        public static async Task<string> GetAsync(string uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using var response = (HttpWebResponse) await request.GetResponseAsync();
            await using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        ///     Цикл запроса обновления истории сообщений
        /// </summary>
        public static async void GetHistoryMessages()
        {
            while (true)
            {
                await UpdateHistory();

                Thread.Sleep(ConfigManager.Config.MillisecondsSleep);
            }
        }

        /// <summary>
        ///     Функция обновления сообщений
        /// </summary>
        public static async Task UpdateHistory()
        {
            var res = await GetAsync($"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Chat");

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
    }
}