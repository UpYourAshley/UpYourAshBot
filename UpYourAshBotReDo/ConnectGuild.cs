using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Discord;
using Discord.WebSocket;
using UpYourAshBotReDo.Core.UserAccounts;
using UpYourAshBotReDo.Core.CustomRoles;

namespace UpYourAshBotReDo
{
    public class ConnectGuild
    {
        public string guildFolder = "";

        public SocketGuild guild;

        public GuildConfig config;

        public UserAccounts accounts;

        public CustomRoles customRoles;

        //public Events events;

        public ConnectGuild(GuildConfig savedConfig, List<SocketGuild> socketGuilds)
        {
            LoggingUtils.LogInfo($"Called method for loading custom info for guild: {savedConfig.name}");
            config = savedConfig;
            LoggingUtils.LogInfo($"Loading accounts for guild: {savedConfig.name}, ID: {savedConfig.id}");
            accounts = new UserAccounts(savedConfig.userAccountPath);
            customRoles = new CustomRoles(savedConfig.customRolePath);
            //events = new Events(config.eventsPath);

            Console.WriteLine($"accounts path in config {savedConfig.userAccountPath}, ACTUALY FILE: ");
        }

        public ConnectGuild(string _guildFolder, SocketGuild _guild, ulong _Id)
        {
            LoggingUtils.LogProcess("Making new Connected Guild Object.");

            //Sets file path
            try
            {
                guildFolder = (_guildFolder + "/" + _Id.ToString());
            }
            catch (Exception e)
            {
                LoggingUtils.LogError(e.Message);
            }

            LoggingUtils.LogInfo($"With Path : {guildFolder}");

            //Sets guild
            guild = _guild;

            LoggingUtils.LogProcess("Making new Connected Guild Config");
            //Makes a new config file
            config = new GuildConfig();
            config.id = guild.Id;
            config.name = guild.Name;
            config.filePath = (guildFolder + "/config.json");
            config.userAccountPath = (guildFolder + "/accounts.json");
            config.customRolePath = (guildFolder + "/customRoles.json");
            config.eightBallResultsPath = (guildFolder + "/8BallAnswers.json");
            config.workOptionsPath = (guildFolder + "/WorkItems.json");
            config.eventsPath = (guildFolder + "/Events.json");
            config.workCoolDown = 5;
            config.workCoolDown = 5;
            LoggingUtils.LogInfo("Checking");
            LoggingUtils.LogInfo($"ID: {config.id} - Name: {config.name}");

            LoggingUtils.LogProcess("Making Guild Directory");
            //Makes new dir for the guild
            try
            {
                Directory.CreateDirectory(guildFolder);
                LoggingUtils.LogInfo("Done");
            }
            catch (Exception e)
            {
                LoggingUtils.LogError(e.Message);
            }

            LoggingUtils.LogProcess("Making Config File");

            File.Create(guildFolder + "/config.json").Close();

            LoggingUtils.LogProcess("Converting to json");
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);

            LoggingUtils.LogProcess("Saving to file");
            File.WriteAllText(guildFolder + "/config.json", json);
            LoggingUtils.LogInfo("Done");

            LoggingUtils.LogProcess("Setting up accounts");
            accounts = new UserAccounts(config.userAccountPath);

            LoggingUtils.LogProcess("Setting up custom Roles");
            customRoles = new CustomRoles(config.customRolePath);

            LoggingUtils.LogProcess("Setting Up Work Items");
            string items = File.ReadAllText("SystemLang/WorkItems.json");
            File.WriteAllText(config.workOptionsPath, items);

            LoggingUtils.LogProcess("Setting Up 8Ball Answers");
            string answers = File.ReadAllText("SystemLang\\8BallAnswers.json");
            File.WriteAllText(config.eightBallResultsPath, answers);

            //LoggingUtils.LogProcess("Setting Up Events");
            //events = new Events(config.eventsPath);

        }

        public void SaveConfig(GuildConfig newConfig)
        {
            string json = JsonConvert.SerializeObject(newConfig, Formatting.Indented);

            File.WriteAllText(config.filePath, json);
        }
    }
}
