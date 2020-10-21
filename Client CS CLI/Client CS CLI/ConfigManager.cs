using System.IO;
using Newtonsoft.Json;

namespace Client_CS_CLI
{
    internal class ConfigManager
    {
        protected static Config config = new Config();

        /// <summary>
        /// <para>Функция загрузки данных о пользователе из конфиг файла json</para>
        /// <br>по стандарту: {"MillisecondsSleep":200,"AskNick":true,"Name":"anonymous"}</br>
        /// </summary>
        protected static void LoadConfig()
        {
            var path = @"config.json";

            if (!File.Exists(path))
            {
                using (var streamWriter = new StreamWriter(path))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(config));
                }

                return;
            }

            using (var streamReader = new StreamReader(path))
            {
                config = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
            }
        }
    }
    /// <summary>
    /// <para>Класс объекта json с конфигурационными данными</para>
    /// <br>MillisecondsSleep - время обновления</br>
    /// <br>AskNick - запрашивать ли ник при входе</br>
    /// <br>Name - имя по стандарту при отсутствии ника</br>
    /// </summary>
    internal class Config
    {
        public int MillisecondsSleep { get; set; } = 200;
        public bool AskNick { get; set; } = true;
        public string Name { get; set; } = "anonymous";

    }
}