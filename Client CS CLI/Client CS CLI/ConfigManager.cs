using System.IO;
using Newtonsoft.Json;

namespace Client_CS_CLI
{
    internal class ConfigManager
    {
        protected static Config config = new Config();

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
    internal class Config
    {
        public int MillisecondsSleep { get; set; } = 200;
        public bool AskNick { get; set; } = true;
        public string Name { get; set; } = "anonymous";

    }
}