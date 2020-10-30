using System.IO;
using Newtonsoft.Json;

namespace Client_CS_CLI
{
    internal class ConfigManager
    {
        private const string Path = @"config.json";
        protected static Config Config = new Config();

        public static async void WriteConfig()
        {
            await using var streamWriter = new StreamWriter(Path);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(Config));
        }

        /// <summary>
        ///     <para>Функция загрузки данных о пользователе из конфиг файла json</para>
        ///     <br>по стандарту: {"MillisecondsSleep":200,"AskNick":true,"Name":"anonymous"}</br>
        /// </summary>
        protected static async void LoadConfig()
        {
            if (!File.Exists(Path)) WriteConfig();

            using var streamReader = new StreamReader(Path);
            Config = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
        }
    }

    /// <summary>
    ///     <para>Класс объекта json с конфигурационными данными</para>
    ///     <br>MillisecondsSleep - время обновления</br>
    ///     <br>AskNick - запрашивать ли ник при входе</br>
    ///     <br>Name - имя по стандарту при отсутствии ника</br>
    /// </summary>
    internal class Config
    {
        public int MillisecondsSleep { get; set; } = 200;
        public RegData RegData { get; set; } = new RegData {Username = "Anonymous", Password = "password"};
        public string Token { get; set; }
    }

    public class RegData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}