using System;
using System.IO;
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
            var storageFolder =
                ApplicationData.Current.LocalFolder;
            var sampleFile =
                await storageFolder.CreateFileAsync(Path,
                    CreationCollisionOption.ReplaceExisting);

            var stream = await sampleFile.OpenStreamForWriteAsync();
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync(JsonConvert.SerializeObject(Config));
            }
        }

        public static async void LoadConfig()
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
    }
}