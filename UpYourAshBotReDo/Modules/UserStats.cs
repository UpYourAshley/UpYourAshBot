using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using UpYourAshBotReDo.Core.UserAccounts;


namespace UpYourAshBotReDo.Modules
{
    public class UserStats : ModuleBase<SocketCommandContext>
    {
        [Command("LeaderBoard")]
        public async Task LeaderBoard()
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);
            List<UserAccount> sortedAccountsAllTime = connectGuild.accounts.GetSortedAccountsAllTime();
            List<UserAccount> sortedAccountsPastWeek = connectGuild.accounts.GetSortedAccountsWeek();

            //ALL TIME
            EmbedBuilder embedAll = new EmbedBuilder();

            int added = 0;
            foreach(UserAccount a in sortedAccountsAllTime)
            {
                added++;
                if(added < 6)
                {
                    embedAll.AddField($"{added}", $"**Name**: {a.Name}, **XP**: {a.XP}, **Level** :{a.LevelNumber}");
                }
            }


            

            embedAll.WithTitle("Leader Board - All Time")
               .WithDescription("**Leveling LeaderBoard All Time**")
               .WithFooter(x =>
               {
                   x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                   x.IconUrl = Context.User.GetAvatarUrl();
               })
                .WithColor(Utilities.GetColor());

            //PAST WEEK
            EmbedBuilder embedWeek = new EmbedBuilder();

            added = 0;
            foreach (UserAccount a in sortedAccountsPastWeek)
            {
                added++;
                if (added < 6)
                {
                    embedWeek.AddField($"{added}", $"**Name**: {a.Name}, **XP**: {a.WeekXP}, **Level** :{a.LevelNumber}");
                }
            }


            embedWeek.WithTitle("Leader Board - Past Week")
               .WithDescription("**Leveling LeaderBoard - Past Week**")
               .WithFooter(x =>
               {
                   x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                   x.IconUrl = Context.User.GetAvatarUrl();
               })
                .WithColor(Utilities.GetColor());

            await Context.Channel.SendMessageAsync("", embed: embedAll.Build());
            await Context.Channel.SendMessageAsync("", embed: embedWeek.Build());
        }

        [Command("Stats")]
        public async Task Stats([Remainder] string user = "optional")
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            SocketUser member = Context.Message.MentionedUsers.FirstOrDefault();
            
            if(member == null)
            {

                var result = from a in Context.Guild.Users
                             where (a.Nickname ?? a.Username).ToLower() == user.ToLower()
                             select a;

                member = result.FirstOrDefault();
            }
            SocketUser target = member ?? Context.User;
            SocketGuildUser guildTarget = (SocketGuildUser)target;
            var userAccount = connectGuild.accounts.GetAccount(target);

            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle(target.Username + "'s Stats")
                .WithColor(61, 61, 61)
                .WithDescription(target.Username + "'s stats are:")
                .AddField("XP: ", userAccount.XP)
                .AddField("XP This Week", userAccount.WeekXP)
                .AddField("Level: ", userAccount.LevelNumber)
                .AddField("Join Date: ", guildTarget.JoinedAt)
                .AddField("Is Muted: ", userAccount.IsMuted)
                .AddField("Last Message:", userAccount.LastMessage.ToShortDateString())
                .AddField("Server Name:", Context.Guild.Name)
                .WithFooter(x =>
                {
                    x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                    x.IconUrl = Context.User.GetAvatarUrl();
                })
                .WithThumbnailUrl(target.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }
    }
}
