using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UpYourAshBotReDo.Modules
{
    public class Fun : ModuleBase<SocketCommandContext>
    {
        private static Random r = new Random();

        [Command("Echo")]
        [RequireContext(ContextType.Guild)]
        public async Task Echo([Remainder]string text)
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(text, false);
        }

        [Command("8Ball")]
        public async Task RNG([Remainder]string question)
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            string json = File.ReadAllText(connectGuild.config.eightBallResultsPath);

            List<string> answers = JsonConvert.DeserializeObject<List<string>>(json);

            string selection = answers[r.Next(0, answers.Count)];

            await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed($"8Ball for {Context.User.Username}", $"You said: '{question}'", "**Answer:**", selection, Context));
        }

        [Command("Pick")]
        public async Task Pick([Remainder]string options)
        {
            string[] splitVars = options.Split('|');

            if(splitVars.Length < 2)
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"Please have more than one option, seperated by '|'", Context));
                return;
            }

            string option = splitVars[r.Next(0, splitVars.Length)];

            await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Random Selection!", $"The option picked was: {option}", Context));
        }
    }
}
