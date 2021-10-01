using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Newtonsoft.Json;
using UpYourAshBotReDo.Core.AutomaticEvents;

namespace UpYourAshBotReDo.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {

        [Command("Help")]
        public async Task Help([Remainder]string var1 = "1")
        {
            bool isCommand = false;
            foreach(string cat in BotVars.botVars.helpCats)
            {
                if(var1.ToLower() == cat.ToLower())
                {
                    isCommand = true;
                }
            }

            if (isCommand)
            {
                Dictionary<string, string> commands = BotVars.botVars.catCommands[var1.ToLower()];

                string pageNum = "1";
                int startIndex = (Convert.ToInt32(pageNum) - 1) * 10;
                int endIndex = startIndex + 10;
                EmbedBuilder embed = new EmbedBuilder();

                int numPages = (int)Math.Ceiling((double)commands.Keys.Count / 10);
                if (numPages < 1) numPages = 1;

                //REDO IF TEHRE ARE MROE THAN 10 COMMANDS PER CAT
                foreach(string key in commands.Keys)
                {
                    embed.AddField($"Command: {Config.bot.cmdPrefix}{key}", $"{commands[key]}");
                }


                embed.WithTitle($"{var1} - Commands")
                    .WithDescription("For paramters, optional ones are marked with * - **Seperate paramters with '|'** - This is still in development :)")
                    .AddField("Page Num - ", $"{pageNum}/{numPages}")
                    .WithColor(Utilities.GetColor());

                await Context.Channel.SendMessageAsync("", embed: embed.Build());

                return;
            }
            //Isnt getting hel pfor cat
            else
            {
                string pageNum = var1;
                int startIndex = (Convert.ToInt32(pageNum) - 1) * 10;
                int endIndex = startIndex + 10;
                EmbedBuilder embed = new EmbedBuilder();

                //int numPages = connectGuild.customRoles.customRoles.Count / 10;
                int numPages = (int)Math.Ceiling((double)BotVars.botVars.helpCats.Count / 10);
                if (numPages < 1) numPages = 1;

                if (Convert.ToInt32(var1) > numPages)
                {
                    await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"There is no page {var1}.", Context));
                    return;
                }

                //max items per page = 10
                for (int i = startIndex; i != endIndex; i++)
                {
                    if (BotVars.botVars.helpCats.Count > i)
                    {
                        embed.AddField($"{i})", $"**{BotVars.botVars.helpCats[i]}**");
                    }
                }


                embed.WithTitle("Help Categories")
                    .WithDescription("To get help on a category do |help <category> - To see other pages do |help <pageNum> - This is still in development :)")
                    .AddField("Page Num - ", $"{pageNum}/{numPages}");

                await Context.Channel.SendMessageAsync("", embed: embed.Build());
            }
        }

        //[Command("UserInfo")]
        //public async Task UserInfo([Remainder] string user = "optional")
        //{
        //    ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

        //    SocketUser member = Context.Message.MentionedUsers.FirstOrDefault();

        //    if (member == null)
        //    {

        //        var result = from a in Context.Guild.Users
        //                     where (a.Nickname ?? a.Username) == user
        //                     select a;

        //        member = result.FirstOrDefault();
        //    }
        //    SocketUser target = member ?? Context.User;
        //    SocketGuildUser guildTarget = (SocketGuildUser)target;

        //    await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("User Info", $"Name: {guildTarget.Username}, ID: {guildTarget.Id}", Context));
        //}
        
        [Command("SendAll")]
        public async Task SendAll([Remainder]string message)
        {
            if(Context.User.Id != 339071846651527169)
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Permission Error", "Only the bot maker can use this.", Context));
                return;
            }
            foreach(ConnectGuild guild in Program._guildManager.connectedGuilds)
            {
                SocketGuild sG = Context.Client.GetGuild(guild.config.id);
                await sG.DefaultChannel.SendMessageAsync("", embed: Utilities.EasyEmbed("Bot Developer Announcement", message, Context));
            }
        }

        [Command("Roles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Roles()
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            List<SocketRole> Roles = Context.Guild.Roles.ToList();

            string rolesString = "";
            foreach(SocketRole r in Roles)
            {
                rolesString = rolesString + ($"\nName: {r.Name} Colour: {r.Color}");
            }
            rolesString = rolesString.Replace("@everyone", "everyone");

            await Context.Channel.SendMessageAsync($"WIP - There are {Roles.Count} roles. - {rolesString}", false);
        }

        [Command("RoleInfo")]
        [RequireUserPermission(GuildPermission.AddReactions)]
        public async Task GetRoleInfo([Remainder] string name)
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);
            List<SocketRole> Roles = Context.Guild.Roles.ToList();

            var result = from a in Roles
                         where a.Name.ToLower() == name.ToLower()
                         select a;
            SocketRole role = result.FirstOrDefault();

            if(role == null)
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"Could not find role with the name {name}, sorry.", Context));
                return;
            }
            else
            {
                var embed = new EmbedBuilder();

                embed.WithColor(Utilities.GetColor())
                    .WithTitle("Role Info")
                    .AddField("Name", role.Name)
                    .AddField("ID", role.Id)
                    .AddField("Members", role.Members.Count())
                    .WithFooter(x =>
                    {
                        x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                        x.IconUrl = Context.User.GetAvatarUrl();
                    });

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }



        //[Command("TestReaction")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        //public async Task TestReaction()
        //{
        //    RestUserMessage lastMSG = await Context.Channel.SendMessageAsync("Touch Me");

        //    var tickBox = new Emoji("✅");
        //    var x = new Emoji("❎");

        //    await lastMSG.AddReactionAsync(tickBox);
        //    await lastMSG.AddReactionAsync(x);

        //    //Global.watchMessages.Add(Context.Guild.Id, lastMSG.Id);
        //    Global.watchMessages.Add(lastMSG.Id, Context.Guild.Id);
        //}

        //[Command("Test")]
        //public async Task Test()
        //{
        //    RestUserMessage lastMSG = await Context.Channel.SendMessageAsync("You good?");
        //    Console.WriteLine(lastMSG.Id);

        //    List<dynamic> args = new List<dynamic>();
        //    args.Add("Are you good?");
        //    args.Add("✅");
        //    args.Add("❎");
        //    args.Add(Context);
        //    ReactionEvent newEvent = new ReactionEvent()
        //    {
        //        startTime = DateTime.Now,
        //        messageID = lastMSG.Id,
        //        timeOut = 10,
        //        onTriggeredCall = ReactionEventActions.Ask,
        //        args = args
        //    };


        //    var tickBox = new Emoji("✅");
        //    var x = new Emoji("❎");

        //    await lastMSG.AddReactionAsync(tickBox);
        //    await lastMSG.AddReactionAsync(x);
        //    //Global.watchMessages.Add(Context.Guild.Id, lastMSG.Id);
        //    Global.watchMessages.Add(lastMSG.Id, Context.Guild.Id);
        //    //ReactionEventActions.Test();


        //}
        
            [Command("Join")]
        public async Task ADUIO(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            var connection = await channel.ConnectAsync();
            Console.WriteLine("JOINED");
            SocketVoiceChannel audioClient = (SocketVoiceChannel)connection;

            Program._connections[audioClient.Guild.Id] = connection;

            await Say(connection, "why.mp3");
        }

        private async Task Say(IAudioClient connection, string path)
        {
            try
            {
                await connection.SetSpeakingAsync(true); // send a speaking indicator

                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-i ""{path}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var ffmpeg = Process.Start(psi);

                var output = ffmpeg.StandardOutput.BaseStream;
                var discord = connection.CreatePCMStream(AudioApplication.Voice);
                await output.CopyToAsync(discord);
                await discord.FlushAsync();

                await connection.SetSpeakingAsync(false); // we're not speaking anymore
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"- {ex.StackTrace}");
            }
        }




        [Command("DRAW")]
        public async Task Draw([Remainder]string message = "F U")
        {
            Stopwatch stopwatch = new Stopwatch();


            string url = Context.User.GetAvatarUrl();

            System.Drawing.Image img = DownloadImage(url);

            System.Drawing.Image done = DrawText(message, new Font(FontFamily.GenericSansSerif, 8), System.Drawing.Color.White, System.Drawing.Color.Black, img);

            done.Save($"{Context.User.Username}.png");

            await Context.Channel.SendFileAsync($"{Context.User.Username}.png");

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            await Context.Channel.SendMessageAsync($"That took me: {ts.Hours}:{ts.Minutes}:{ts.Seconds}:{ts.Milliseconds}", false);
        }

        System.Drawing.Image DownloadImage(string fromUrl)
        {
            using (WebClient webClient = new System.Net.WebClient())
            {
                using (Stream stream = webClient.OpenRead(fromUrl))
                {
                    return System.Drawing.Image.FromStream(stream);
                }
            }
        }

        private static System.Drawing.Image DrawText(String text, Font font, System.Drawing.Color textColor, System.Drawing.Color backColor, System.Drawing.Image image)
        {
            //first, create a dummy bitmap just to get a graphics object
            System.Drawing.Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            //img = new Bitmap((int)textSize.Width, (int)textSize.Height);
            //img = new Bitmap(1080, 1080);
            img = image;

            drawing = Graphics.FromImage(img);
            drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //paint the background
            //drawing.Clear(System.Drawing.Color.Transparent);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            //formatting
            StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};

            drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            //drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.DrawString(text, font, textBrush, new Rectangle(0, 0, img.Width, img.Height), sf);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();


            return img;

        }
    }
}
