using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Server_CS
{
    internal class JsonWorker
    {
        /// <summary>
        ///     Файл для сохранения сообщений в виде json массива
        /// </summary>
        private static readonly string path = @"messages.json";

        /// <summary>
        ///     Функция сохранения в файл
        /// </summary>
        /// <param name="messages"></param>
        public static void Save(List<Message> messages)
        {
            using var streamWriter = new StreamWriter(path);
            streamWriter.Write(JsonConvert.SerializeObject(messages));
        }

        /// <summary>
        ///     Функция загрузки сообщений из файла, десирилизованный из json в List <Message> объект
        /// </summary>
        /// <returns></returns>
        public static List<Message> Load()
        {
            var messages = new List<Message>();
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