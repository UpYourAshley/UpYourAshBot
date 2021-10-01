using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace UpYourAshBotReDo
{
    class Config
    {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";

        public static BotConfig bot;

        static Config()
        {
            //will check if thee is alreay a config folder
            if (!Directory.Exists(configFolder))
            {
                //if there is not it wil make one
                Directory.CreateDirectory(configFolder);
            }

            //wll check if there is a config file
            if (!File.Exists(configFolder + "/" + configFile))
            {
                //will make one
                bot = new BotConfig();

                //Serializes it to text
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);

                //Writes it to a file
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            //IF there is a config file
            else
            {
                //Loads it
                string json = File.ReadAllText(configFolder + "/" + configFile);

                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        public string token;
        public string cmdPrefix;
        public string eventCalandarID;
    }
}
