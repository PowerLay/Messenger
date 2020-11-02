using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Newtonsoft.Json;

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Класс управления настройками
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        ///     Путь к файлу настроек
        /// </summary>
        private const string Path = @"config.json";

        /// <summary>
        ///     Объект настроек
        /// </summary>
        public static Config Config = new Config();

        /// <summary>
        ///     Сохранение настроек в файл
        /// </summary>
        public static async void WriteConfig()
        {
            // Create sample file; replace if exists.
            StorageFile sampleFile;
            try
            {
                var storageFolder =
                    ApplicationData.Current.LocalFolder;
                sampleFile =
                    await storageFolder.CreateFileAsync(Path,
                        CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception)
            {
                return;
            }

            var stream = await sampleFile.OpenStreamForWriteAsync();
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync(JsonConvert.SerializeObject(Config));
            }
        }

        /// <summary>
        ///     Чтение настроек из файла
        /// </summary>
        public static async Task LoadConfig()
        {
            var text = "";
            try
            {
                var storageFolder =
                    ApplicationData.Current.LocalFolder;
                var sampleFile =
                    await storageFolder.GetFileAsync(Path);
                text = await FileIO.ReadTextAsync(sampleFile);
            }
            catch (Exception)
            {
                WriteConfig();
            }

            if (text != "")
                Config = JsonConvert.DeserializeObject<Config>(text);

            if (Config == null) Config = new Config();
        }
    }

    /// <summary>
    ///     Класс настроек
    /// </summary>
    public class Config
    {
        /// <summary>
        ///     MillisecondsSleep - время обновления истории сообщений
        /// </summary>
        public int MillisecondsSleep { get; set; } = 200;

        /// <summary>
        ///     Логин пароль пользователя
        /// </summary>
        public RegData RegData { get; set; } = new RegData {Username = "Anonymous", Password = "password"};

        /// <summary>
        ///     Размер окна приложения
        /// </summary>
        public Size Size { get; set; } = new Size(480, 800);

        /// <summary>
        ///     Токен необходимый для отправки сообщений
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     Адрес сервера
        /// </summary>
        public string IP { get; set; } = "localhost";

        /// <summary>
        ///     Порт сервера
        /// </summary>
        public int Port { get; set; } = 5000;
    }

    /// <summary>
    ///     Класс для хранения связки логин/пароль
    /// </summary>
    public class RegData
    {
        /// <summary>
        ///     Логин уникальный псевдоним пользователя
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Пароль - необходим для доступа
        /// </summary>
        public string Password { get; set; }
    }
}