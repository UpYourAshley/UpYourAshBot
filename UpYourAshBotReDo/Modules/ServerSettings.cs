using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UpYourAshBotReDo.Modules
{
    public class ServerSettings : ModuleBase<SocketCommandContext>
    {
        public string[] on = new string[] { "on", "true", "enabled", "enable" };
        public string[] off = new string[] { "off", "false", "disabled", "disable" };

        [Command("ServerInfo")]
        public async Task ServerInfo()
        {
            int bots = 0;
            int people = 0;

            foreach(SocketGuildUser e in Context.Guild.Users)
            {
                if (e.IsBot)
                {
                    bots++;
                }
                else
                {
                    people++;
                }
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder.WithTitle("Server Settings")
                .WithColor(Utilities.GetColor())
                .AddField("Name", Context.Guild.Name)
                .AddField("Members", Context.Guild.MemberCount)
                .AddField("People", people)
                .AddField("Bots", bots)
                .AddField("ID", Context.Guild.Id)
                .AddField("Created At", Context.Guild.CreatedAt)
                .WithFooter(x =>
                {
                    x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                    x.IconUrl = Context.User.GetAvatarUrl();
                })
                .WithThumbnailUrl(Context.Guild.SplashUrl);

            await Context.Channel.SendMessageAsync("", embed: embedBuilder.Build());
        }


        [Command("ServerSettings")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ShowServerSettings()
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder.WithTitle("Server Settings")
                .WithColor(Utilities.GetColor())
                .AddField("User Join Message", connectGuild.config.joinMessage ?? "Not Set")
                .AddField("User Left Message", connectGuild.config.userLeftMessage ?? "Note Set")
                .AddField("Show Join Message", connectGuild.config.userJoinedNotifaction)
                .AddField("Show Leave Message", connectGuild.config.userLeftNotifaction)
                .WithFooter(x =>
                {
                    x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                    x.IconUrl = Context.User.GetAvatarUrl();
                })
                .WithThumbnailUrl(Context.Guild.SplashUrl);

            await Context.Channel.SendMessageAsync("", embed: embedBuilder.Build());

        }

        #region Server Settings

        [Command("LevelUpMessage")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task LevelUpMessage(string var = "")
        {

            ConnectGuild server = Program._guildManager.GetGuild(Context.Guild);

            //checks is a paramter was passed
            if (String.IsNullOrEmpty(var))
            {
                //Is empty
                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Level Up Message Status", $"Level up message enabled: **{server.config.levelUpMessage.ToString()}**", Context));

                return;
            }

            if (on.Contains<String>(var.ToLower()))
            {
                server.config.levelUpMessage = true;
            }
            else if (off.Contains<String>(var.ToLower()))
            {
                server.config.levelUpMessage = false;
            }
            else
            {
                //wrong 
                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Error", "Invalid value given.", Context));
                return;
            }

            server.SaveConfig(server.config);

            await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Setting Changed", $"Level up message is now set to **{server.config.levelUpMessage.ToString()}**", Context));

        }

        [Command("LogChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task LogChannel(IGuildChannel channel = null)
        {
            ConnectGuild server = Program._guildManager.GetGuild(Context.Guild);

            //checks is a paramter was passed
            if (channel == null)
            {
                //Is empty

                string name = null;
                //Checks if set
                if(server.config.logChannelID != 0)
                {
                    name = Context.Guild.GetChannel(server.config.logChannelID).Name;
                }

                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Log Channel", $"Log Channel: **{name ?? "Note Set"}**", Context));

                return;
            }

            server.config.logChannelID = channel.Id;

            server.SaveConfig(server.config);

            await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Settings Changed!", $"Bot log channel is now **{channel.Name}**", Context));
        }

        [Command("UserJoinMessage")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UserJoinMessage([Remainder]string var = "")
        {
            ConnectGuild server = Program._guildManager.GetGuild(Context.Guild);

            //checks if var was apssed
            if (string.IsNullOrEmpty(var))
            {
                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("User Join Message", $"Enabled: **{server.config.showJoinMessage.ToString()}**, Message: **{server.config.joinMessage ?? "Not Set"}**", Context));
            }
            else if(on.Contains<String>(var.ToLower()))
            {
                server.config.showJoinMessage = true;
                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("User Join Message Updated", $"Enabled: **{server.config.showJoinMessage.ToString()}**, Message: **{server.config.joinMessage ?? "Not Set"}**", Context));
                server.SaveConfig(server.config);
            }
            else if (off.Contains<String>(var.ToLower()))
            {
                server.config.showJoinMessage = false;
                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("User Join Message Updated", $"Enabled: **{server.config.showJoinMessage.ToString()}**, Message: **{server.config.joinMessage ?? "Not Set"}**", Context));
                server.SaveConfig(server.config);
            }
            else
            {
                server.config.joinMessage = var;
                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("User Join Message Updated", $"Enabled: **{server.config.showJoinMessage.ToString()}**, Message: **{server.config.joinMessage}**", Context));
                server.SaveConfig(server.config);
            }
        }

        [Command("UserVerification")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UserVerification([Remainder] string vars = "")
        {
            ConnectGuild server = Program._guildManager.GetGuild(Context.Guild);

            if (string.IsNullOrEmpty(vars))
            {
                SocketRole role = null;
                if (server.config.userVerificationRoleID != 0) role = Context.Guild.GetRole(server.config.userVerificationRoleID);

                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("User Verification", $"Enabled: {server.config.userVerification} \nRole: ", Context));
            }
        }

        [Command("UserVerificationRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UserVerificationRole(SocketRole role = null)
        {
            ConnectGuild server = Program._guildManager.GetGuild(Context.Guild);

            //checks is a paramter was passed
            if (role == null)
            {
                //Is empty

                string name = null;
                //Checks if set
                if (server.config.userVerificationRoleID != 0)
                {
                    name = Context.Guild.GetRole(server.config.userVerificationRoleID).Name;
                }

                await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Verified User Role", $"Role: **{name ?? "Note Set"}**", Context));

                return;
            }

            server.config.userVerificationRoleID = role.Id;

            server.SaveConfig(server.config);

            await Context.Channel.SendMessageAsync("", false, Utilities.EasyEmbed("Settings Changed!", $"Verified User Role: **{role.Name}**", Context));
        }

        #endregion

        #region Helpers

        #endregion
    }
}
