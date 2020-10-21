using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CLient_CS_UWP
{
    public class ConfigManager
    {
        public static Config config = new Config();

        public static void LoadConfig()
        {
            var path = @"C:\config.json";

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
    public class Config
    {
        public int MillisecondsSleep { get; set; } = 200;
        public bool AskNick { get; set; } = true;
        public string Name { get; set; } = "anonymous";

    }
}
