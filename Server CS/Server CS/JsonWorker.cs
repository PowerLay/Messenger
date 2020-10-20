using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Server_CS
{
    class JsonWorker
    {
        private static string path = @"messages.json";
        public static void Save(List<Message> messages)
        {
            using var streamWriter = new StreamWriter(path);
            streamWriter.Write(JsonConvert.SerializeObject(messages));
        }

        public static List<Message> Load()
        {
            List<Message> messages = new List<Message>();
            if (!File.Exists(path))
                return messages;
            using (var streamReader = new StreamReader(path))
            {
               messages = JsonConvert.DeserializeObject<List<Message>>(streamReader.ReadToEnd());
            }

            return messages;
        }
    }
}