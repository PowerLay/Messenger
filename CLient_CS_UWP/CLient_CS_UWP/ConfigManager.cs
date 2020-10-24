using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace CLient_CS_UWP
{
    public class ConfigManager
    {
        public static Config Config = new Config();

        private static async void WriteConfig(string path)
        {
            // Create sample file; replace if exists.
            var storageFolder =
                ApplicationData.Current.LocalFolder;
            var sampleFile =
                await storageFolder.CreateFileAsync(path,
                    CreationCollisionOption.ReplaceExisting);
            //var sf = await Package.Current.InstalledLocation.TryGetItemAsync(path) as StorageFile;

            var stream = await sampleFile.OpenStreamForWriteAsync();
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync(JsonConvert.SerializeObject(Config));
            }
        }

        public static async void LoadConfig()
        {
            var path = @"config.json";
            var text = "";
            try
            {
                var storageFolder =
                    ApplicationData.Current.LocalFolder;
                var sampleFile =
                    await storageFolder.GetFileAsync(path);
                text = await FileIO.ReadTextAsync(sampleFile);
            }
            catch (Exception)
            {
                WriteConfig(path);
            }

            if (text != "")
                Config = JsonConvert.DeserializeObject<Config>(text);

            if (Config == null) Config = new Config();
        }
    }

    public class Config
    {
        public int MillisecondsSleep { get; set; } = 200;
        public bool AskNick { get; set; } = true;
        public string Name { get; set; } = "anonymous";
    }
}