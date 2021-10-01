using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using UpYourAshBotReDo.APIs;
using UpYourAshBotReDo.APIs.Urban_Dict;

namespace UpYourAshBotReDo.Modules
{
    public class UrbanDict : ModuleBase<SocketCommandContext>
    {
        [Command("Define"), Alias("Def", "Definition")]
        public async Task Define([Remainder]string word)
        {
            UrbanDictClass response;
            response = await APIManager.GetDef(word);

            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle($"Word Definition: **{word}**")
                .WithColor(Utilities.GetColor())
                .WithDescription("But why")
                .AddField("Description: ", response.list[0].definition)
                .AddField("Author: ", response.list[0].author)
                .WithFooter(x =>
                {
                    x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp.LocalDateTime}";
                    x.IconUrl = Context.User.GetAvatarUrl();
                });

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
