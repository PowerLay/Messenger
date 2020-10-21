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
            string nick;
            if (config.AskNick)
            {
                Console.Write("Enter your nick> ");
                nick = Console.ReadLine();
            }
            else
            {
                nick = config.Name;
                Console.WriteLine($"Your nick: {nick}. (Check config.json)");
            }

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
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static int _len = 0;
        private static async Task GetHistoryMessages()
        {
            while (true)
            {
                await UpdateHistory();

                Thread.Sleep(config.MillisecondsSleep);
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

                    string history="";
                    foreach (var message in messages) history += message.ToString().PadRight(Console.BufferWidth-1)+"\n";
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine(history);
                    Console.SetCursorPosition(x, messages.Count + 1);

                    _len = messages.Count;
                }
            }
        }

        private static void Post(string nick)
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

            SendMessage(nick,msg, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        private static void GetAnswer(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            if (result != "ok") Console.WriteLine("Something went wrong");
        }

        private static void SendMessage(string nick, string msg, HttpWebRequest httpWebRequest)
        {
            var json = JsonConvert.SerializeObject(new Message { Name = nick, Msg = msg });
            using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }

}