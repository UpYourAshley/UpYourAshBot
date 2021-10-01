using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace UpYourAshBotReDo
{
    public class GuildManager
    {
        private const string guildsFolder = "Resources/Guilds";

        public List<ConnectGuild> connectedGuilds = new List<ConnectGuild>();
        public int NumOfGuilds
        {
            get
            {
                return connectedGuilds.Count();
            }
        }

        private static DiscordSocketClient _client;

        public GuildManager(DiscordSocketClient client)
        {
            _client = client;
            //Checks if folder 
            if (!Directory.Exists(guildsFolder))
            {
                Directory.CreateDirectory(guildsFolder);
                return;
            }

            //runs through all guilds
            string[] guildFolders = Directory.GetDirectories(guildsFolder);

            //checks if null
            if (guildFolders == null || guildFolders.Count<string>() == 0)
            {
                return;
            }
            else
            {
                LoggingUtils.LogInfo($"Found {guildFolders.Count()} guild folders");

                LoggingUtils.LogProcess("Loading Saved Guilds");
                foreach (string path in guildFolders)
                {
                    GuildConfig config;
                    string configJson = "";

                    LoggingUtils.LogProcess($"Loading Guild With Path {path}");
                    LoggingUtils.LogProcess($"Loading Config File");
                    try
                    {
                        configJson = File.ReadAllText(path + "/config.json");

                    }
                    catch (Exception e)
                    {
                        LoggingUtils.LogError(e.Message);
                    }

                    try
                    {
                        config = JsonConvert.DeserializeObject<GuildConfig>(configJson);
                        ConnectGuild connectGuild = new ConnectGuild(config, _client.Guilds.ToList<SocketGuild>());

                        connectedGuilds.Add(connectGuild);
                    }
                    catch (Exception e)
                    {
                        LoggingUtils.LogError(e.Message);
                    }



                }
            }


        }

        public void CheckOrMakeGuild(SocketGuild guild)
        {
            bool guildFound = false;
            LoggingUtils.LogInfo($"Checking for Guild - {guild.Name}");
            foreach (ConnectGuild a in connectedGuilds)
            {
                if (a.config.id == guild.Id)
                {
                    guildFound = true;
                }

            }

            if (!guildFound)
            {
                LoggingUtils.LogInfo($"New Guild - ID:{guild.Id}");
                ConnectGuild newGild = new ConnectGuild(guildsFolder, guild, guild.Id);
                connectedGuilds.Add(newGild);
                LoggingUtils.LogInfo($"Connected to {connectedGuilds.Count()}");
                return;
            }
            LoggingUtils.LogInfo("Guild Found");
        }

        public ConnectGuild GetGuild(SocketGuild guild)
        {
            var result = from a in connectedGuilds
                         where a.config.id == guild.Id
                         select a;

            return result.FirstOrDefault();
        }

        public SocketGuild GetSocketGuild(ConnectGuild guild)
        {
            var result = from a in _client.Guilds
                         where a.Id == guild.config.id
                         select a;

            return result.FirstOrDefault();
        }
    }
}
