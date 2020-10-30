using System;
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
        private static readonly string MessagesPath = @"messages.json";

        private static readonly string RegDataPath = @"regData.json";

        /// <summary>
        ///     Функция сохранения в файл
        /// </summary>
        /// <param name="messages"></param>
        public static async void Save(List<Message> messages)
        {
            await using var streamWriter = new StreamWriter(MessagesPath);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(messages));
        }

        /// <summary>
        ///     Функция сохранения в файл
        /// </summary>
        /// <param name="messages"></param>
        public static async void Save(List<RegData> regDatas)
        {
            await using var streamWriter = new StreamWriter(RegDataPath);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(regDatas));
        }

        /// <summary>
        ///     Функция загрузки сообщений из файла, десирилизованный из json в List <Message> объект
        /// </summary>
        /// <returns></returns>
        public static async void Load()
        {
            if (!File.Exists(MessagesPath))
                return;
            try
            {
                using var streamReader = new StreamReader(MessagesPath);
                Program.Messages = JsonConvert.DeserializeObject<List<Message>>(await streamReader.ReadToEndAsync());
            }
            catch (Exception)
            {
            }

            if (!File.Exists(RegDataPath))
                return;
            try
            {
                using var streamReader = new StreamReader(RegDataPath);
                Program.RegDatas = JsonConvert.DeserializeObject<List<RegData>>(await streamReader.ReadToEndAsync());
            }
            catch (Exception)
            {
            }
        }
    }
}