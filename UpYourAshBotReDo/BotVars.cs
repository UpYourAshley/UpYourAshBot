using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace UpYourAshBotReDo
{
    class BotVars
    {
        private const string configVarsFolder = "Resources";
        private const string configVarsFile = "configVars.json";

        public static BotConfigVars botVars;

        static BotVars()
        {
            //will check if thee is alreay a config folder
            if (!Directory.Exists(configVarsFolder))
            {
                //if there is not it wil make one
                Directory.CreateDirectory(configVarsFolder);
            }

            //wll check if there is a config file
            if (!File.Exists(configVarsFolder + "/" + configVarsFile))
            {
                //will make one
                botVars = new BotConfigVars();

                botVars.helpCats = new List<string>();
                botVars.catCommands = new Dictionary<string, Dictionary<string, string>>();

                botVars.helpCats.Add("Custom Roles");
                botVars.helpCats.Add("Leveling");

                Dictionary<string, string> customRoleCommands = new Dictionary<string, string>();
                customRoleCommands.Add("ListCustomRoles", "Alies:LCR - Description: Lists all custom roles the server has");
                customRoleCommands.Add("IAm", "Alies:Null - Assigns yourself with that custom roles");
                botVars.catCommands.Add("CustomRoles", customRoleCommands);

                //Serializes it to text
                string json = JsonConvert.SerializeObject(botVars, Formatting.Indented);

                //Writes it to a file
                File.WriteAllText(configVarsFolder + "/" + configVarsFile, json);
            }
            //IF there is a config file
            else
            {
                //Loads it
                string json = File.ReadAllText(configVarsFolder + "/" + configVarsFile);

                botVars = JsonConvert.DeserializeObject<BotConfigVars>(json);
            }
        }

    }

    public struct BotConfigVars
    {
        public List<string> helpCats;
        public Dictionary<string, Dictionary<string, string>> catCommands;
    }
}
