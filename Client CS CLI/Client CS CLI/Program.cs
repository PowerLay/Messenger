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
        private static void Main(string[] args)
        {
            LoadConfig();
            var nick = config.Name;
            Console.WriteLine($"Your nick: {nick}. (Check config.json)");
            while (true)
                try
                {
                    GetHistoryMessages();
                    while (true) Post(nick);
                }
                catch (Exception)
                {
                    // ignored
                }
        }

        public static async Task<string> GetAsync(string uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static async Task GetHistoryMessages()
        {
            var len = 0;
            while (true)
            {
                var res = await GetAsync("http://localhost:5000/api/Chat");

                if (res != "[]")
                {
                    var messages = JsonConvert.DeserializeObject<List<Message>>(res);

                    if (len != messages.Count)
                    {
                        var x = Console.CursorLeft;
                        var y = Console.CursorTop;

                        Console.MoveBufferArea(0, y, x, 1, 0, messages.Count + 1);

                        Console.SetCursorPosition(0, 1);
                        foreach (var message in messages) Console.WriteLine(message);

                        Console.SetCursorPosition(x, messages.Count + 1);

                        len = messages.Count;
                    }
                }

                Thread.Sleep(config.MillisecondsSleep);
            }
        }

        private static void Post(string nick)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create("http://localhost:5000/api/Chat");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            SendMessage(nick, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        private static void GetAnswer(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            using var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            if (result != "ok") Console.WriteLine("Something went wrong");
        }

        private static void SendMessage(string nick, HttpWebRequest httpWebRequest)
        {
            Console.Write("Enter message> ");
            var msg = Console.ReadLine();
            var json = JsonConvert.SerializeObject(new Message {Name = nick, Msg = msg});
            using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }

}