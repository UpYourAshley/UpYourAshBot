using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace UpYourAshBotReDo.Core.LevelingSystem
{
    internal static class Leveling
    {
        internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel, SocketGuild guild)
        {

            Console.WriteLine("MESSAGE GOTTEN");

            ConnectGuild connectGuild = Program._guildManager.GetGuild(guild);

            UserAccounts.UserAccounts userAccounts = Program._guildManager.GetGuild(guild).accounts;

            var userAccount = userAccounts.GetAccount(user);
            uint oldLevel = userAccount.LevelNumber;

            //checks for time out
            if ((DateTime.UtcNow - userAccount.LastMessage.ToUniversalTime()) < TimeSpan.FromMinutes(1))
            {
                return;
            }

            //Debugging Name
            if (user.Username != userAccount.Name)
            {
                userAccount.Name = user.Username;
            }

            userAccount.LastMessage = DateTime.Now;
            userAccount.XP += 50;
            userAccount.WeekXP += 50;
            userAccounts.SaveAccounts();
            uint newLevel = userAccount.LevelNumber;

            if (connectGuild.config.levelUpMessage)
            {
                if (oldLevel != newLevel)
                {
                    //level up
                    var embed = new EmbedBuilder();

                    embed.WithColor(Utilities.GetColor())
                        .WithTitle("Level Up!")
                        .WithDescription(user.Username + " just leveled up!")
                        .AddField("Level", newLevel)
                        .AddField("XP", userAccount.XP);

                    Console.WriteLine("Sending message");
                    await channel.SendMessageAsync("", false, embed.Build());
                }
            }

        }
    }
}
