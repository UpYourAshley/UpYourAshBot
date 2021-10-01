using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpYourAshBotReDo.Core.AutomaticEvents;
using UpYourAshBotReDo.Core.UserAccounts;

namespace UpYourAshBotReDo.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Command("Mute")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task MuteUser([Remainder] string user = "optional")
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);
            UserAccounts userAccounts = connectGuild.accounts;

            SocketUser member = Context.Message.MentionedUsers.FirstOrDefault();

            if (member == null)
            {

                var result = from a in Context.Guild.Users
                             where (a.Nickname ?? a.Username).ToLower() == user.ToLower()
                             select a;

                member = result.FirstOrDefault();
            }

            if(member == null)
            {
                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Error", "Either no user given or failed to find user.", Context));
                return;
            }

            var userAccount = connectGuild.accounts.GetAccount((SocketGuildUser)member);


            userAccount.IsMuted = true;

            var embed = new EmbedBuilder();

            embed.WithTitle("Muted by " + Context.User.Username)
                .WithDescription(member.Mention + " is now muted.")
                .WithFooter(x =>
                {
                    x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                    x.IconUrl = Context.User.GetAvatarUrl();
                })
                .WithThumbnailUrl(member.GetAvatarUrl())
                .WithColor(Utilities.GetColor());

            userAccounts.SaveAccounts();

            await Context.Channel.SendMessageAsync("", embed: embed.Build()) ;
        }

        [Command("Unmute")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task UnmuteUser([Remainder] string user = "optional")
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);
            UserAccounts userAccounts = connectGuild.accounts;

            SocketUser member = Context.Message.MentionedUsers.FirstOrDefault();

            //Checks if a user wasn't pinged
            //Will then find one by username
            if (member == null)
            {

                var result = from a in Context.Guild.Users
                             where (a.Nickname?? a.Username).ToLower() == user.ToLower()
                             select a;

                member = result.FirstOrDefault();
            }

            //Checks that a user was found
            if (member == null)
            {
                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Error", "Either no user given or failed to find user.", Context));
                return;
            }

            var userAccount = connectGuild.accounts.GetAccount((SocketGuildUser)member);


            userAccount.IsMuted = false;

            var embed = new EmbedBuilder();

            embed.WithTitle("Unmuted by " + Context.User.Username)
                .WithDescription(member.Mention + " is now unmuted.")
                .WithFooter(x =>
                {
                    x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                    x.IconUrl = Context.User.GetAvatarUrl();
                })
                .WithThumbnailUrl(member.GetAvatarUrl())
                .WithColor(Utilities.GetColor());

            userAccounts.SaveAccounts();

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        [Command("BanWord")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Banword(string word)
        {

        }

        [Command("UnbanWord")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UnbanWord(string word)
        {

        }

        [Command("ResetLeaderboard")]
        [RequireUserPermission(GuildPermission.AddReactions)]
        public async Task ResetLeaderBoards()
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);
            connectGuild.accounts.ClearWeeklyXP();

            #region setting up reaction event
            RestUserMessage lastMSG = await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Are you sure?", $"Are you sure you want to clear the weekly leaderboard?", Context));
            
            List<Emoji> emojis = new List<Emoji>();
            emojis.Add(new Emoji("✅"));
            emojis.Add(new Emoji("❎"));


            List<dynamic> param = new List<dynamic>();
            param.Add(lastMSG);
            param.Add(emojis);
            param.Add(Context);

            ReactionEventActions.SetUp(lastMSG, emojis, Context);

            ReactionEvent newEvent = new ReactionEvent()
            {
                messageID = lastMSG.Id,
                startTime = DateTime.Now,
                args = param,
                onTriggeredCall = ReactionEventActions.ClearWeeklyLeaderboard,
                timeOut = 10
            };
            #endregion

        }
    }
}
