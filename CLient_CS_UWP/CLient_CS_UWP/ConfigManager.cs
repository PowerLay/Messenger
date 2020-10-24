using System;
using Windows.Foundation;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace CLient_CS_UWP
{
    public class ConfigManager
    {
        private const string Path = @"config.json";
        public static Config Config = new Config();

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

    public class Config
    {
        public int MillisecondsSleep { get; set; } = 200;
        public string Name { get; set; } = "anonymous";
        public Size Size { get; set; } = new Size(480, 800);
    }
}