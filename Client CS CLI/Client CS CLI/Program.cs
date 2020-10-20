using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Client_CS_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter nick> ");
            var nick = Console.ReadLine();
            while (true) Post(nick);

        }
        private static void Post(string nick)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/api/Chat");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            SendMessage(nick, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        private static void GetAnswer(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            if (result != "ok") Console.WriteLine("Something went wrong");
        }

        private static void SendMessage(string nick, HttpWebRequest httpWebRequest)
        {
            Console.Write("Enter message> ");
            var msg = Console.ReadLine();
            var json = JsonConvert.SerializeObject(new Message { Name = nick, Msg = msg });
            using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }
}
