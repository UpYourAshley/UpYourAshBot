using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using UpYourAshBotReDo.Services;
using UpYourAshBotReDo.Core.Time;
using UpYourAshBotReDo.Core.AutomaticEvents;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using Discord.Audio;
using System.Diagnostics;

namespace UpYourAshBotReDo
{
    class Program
    {

        public static GuildManager _guildManager;
        public static DiscordSocketClient _client;

        public static Dictionary<ulong, IAudioClient> _connections = new Dictionary<ulong, IAudioClient>();

        //google
        static string[] Scopes = { CalendarService.Scope.CalendarEvents };
        static string ApplicationName = "Test App";

        UserCredential credentials;
        public static CalendarService calenderService;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            //if no info in config
            if (Config.bot.token == "" || Config.bot.token == null)
            {
                return;
            }


            //using (IServiceProvider services = ConfigureServices())
            //{

            //    var client = services.GetRequiredService<DiscordSocketClient>();

            //    client.Log += LogAsync;
            //    services.GetRequiredService<CommandService>().Log += LogAsync;

            //    //BOT LOGIN STUFF
            //    await client.LoginAsync(TokenType.Bot, Config.bot.token);
            //    await client.StartAsync();

            //    //logic to register commands
            //    await services.GetRequiredService<CommandHandlingService>().InitializeAsync();


            //    await Task.Delay(-1);
            //}

            var services = ConfigureServices();
            

            _client = services.GetRequiredService<DiscordSocketClient>();

            _guildManager = new GuildManager(_client);


            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;



            //BOT LOGIN STUFF
            await client.LoginAsync(TokenType.Bot, Config.bot.token);
            await client.StartAsync();

            #region Extra stuff
            client.GuildAvailable += Client_GuildAvailable;

            _client.ReactionAdded += _client_ReactionAdded;
            _client.UserJoined += _client_UserJoined;
            _client.UserLeft += _client_UserLeft;
            _client.MessageDeleted += _client_MessageDeleted;
            _client.Ready += RepeatingTimer.StartTimer;


            if (BotVars.botVars.helpCats == null)
            {
                LoggingUtils.LogInfo("Help Cats are empty .-.");
            }
            #endregion


            //logic to register commands
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await client.SetGameAsync($"on {_guildManager.connectedGuilds.Count} servers");


            //GOOGLE SHIT

            using(var stream = new FileStream("Resources/credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "Resources/token.json";
                credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);

                stream.Close();
            }

            calenderService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });

            //EventsResource.ListRequest request = calenderService.Events.List("primary");
            //request.TimeMin = DateTime.Now;
            //request.ShowDeleted = false;
            //request.SingleEvents = true;
            //request.MaxResults = 10;
            //request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            //Events events = request.Execute();
            //Console.WriteLine("Upcoming events:");
            //if (events.Items != null && events.Items.Count > 0)
            //{
            //    foreach (var eventItem in events.Items)
            //    {
            //        string when = eventItem.Start.DateTime.ToString();
            //        if (String.IsNullOrEmpty(when))
            //        {
            //            when = eventItem.Start.Date;
            //        }
            //        Console.WriteLine("{0} ({1})", eventItem.Summary, when);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No upcoming events found.");
            //}

            await Task.Delay(-1);


        }

        private async Task _client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            SocketTextChannel channel = (SocketTextChannel)arg2;

            //Checks is server has log channel set
            if(Program._guildManager.GetGuild(channel.Guild).config.logChannelID == 0)
            {
                return;
            }

            ISocketMessageChannel logChannel = (ISocketMessageChannel)channel.Guild.GetChannel(Program._guildManager.GetGuild(channel.Guild).config.logChannelID);

            var msg = await arg1.GetOrDownloadAsync();

            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle("Deleted Message")
                .WithColor(Utilities.GetColor())
                .WithDescription($"A Message was deleted. Sender: **{msg.Author.Username}** - Message: **{msg}** - Channel: **{msg.Channel.Name}**")
                .WithFooter(x =>
                {
                    x.Text = $"{System.DateTime.UtcNow.TimeOfDay}";
                });

            await logChannel.SendMessageAsync("", false, embed.Build());

        }

        //TODO - Sort out message on uer left
        private Task _client_UserLeft(SocketGuildUser arg)
        {
            //TODO - Add user left message
            throw new NotImplementedException();
        }

        //TODO - Sort out message on uer Joined
        private async Task _client_UserJoined(SocketGuildUser arg)
        {
            //TODO - Add user joined message
            //Check if message if set and if a channel is on
            //if eiter is missing send error message in bot log stating the conflict
            ConnectGuild server = _guildManager.GetGuild(arg.Guild);

            //Checks if Legit
            if(!string.IsNullOrEmpty(server.config.joinMessage) && server.config.showJoinMessage)
            {
                //TODO get channel and send message
            }
        }

        private async Task _client_ReactionAdded(Cacheable<IUserMessage, ulong> cach, ISocketMessageChannel channel, SocketReaction reaction)
        {
            IGuildChannel guildChannel = (IGuildChannel)channel;

            ISocketMessageChannel chan = (ISocketMessageChannel)await guildChannel.Guild.GetTextChannelAsync(guildChannel.Id);

            //Check if its a userver message
            ConnectGuild connectGuild = _guildManager.GetGuild((SocketGuild)guildChannel.Guild);
            if (cach.Id == connectGuild.config.userVerificationMessageID)
            {
                //Give user the role
                SocketGuildUser user = (SocketGuildUser)await guildChannel.Guild.GetUserAsync(reaction.UserId);
                await user.AddRoleAsync(guildChannel.Guild.GetRole(connectGuild.config.userVerificationRoleID));
            }


            if (Global.watchMessages.ContainsKey(cach.Id))
            {
                if (Global.watchMessages[cach.Id] == guildChannel.GuildId)
                {
                    SocketGuildUser user = (SocketGuildUser)await guildChannel.Guild.GetUserAsync(reaction.UserId);
                    if (user.IsBot) return;

                    Console.WriteLine("Reaction Added");
                    //if that message is being watched
                    //now to check for any events
                    foreach(ReactionEvent rEvent in ReactionEvent.reactionEvents)
                    {
                        Console.Write("Checking Event message IDS");
                        if(rEvent.messageID == cach.Id)
                        {
                            Console.WriteLine("Reaction On message with event");
                            if (!rEvent.beenTriggered)
                            {
                                //TODO- Pass the user who reacted etc
                                Emoji reactedWith = (Emoji)reaction.Emote;
                                rEvent.reactedWith = reactedWith;
                                rEvent.userReactedID = reaction.UserId;
                                rEvent.Trigger();
                                rEvent.reactedWith = null;
                                rEvent.userReactedID = 0;
                            }
                        }
                    }

                    

                }
            }

        }

        private async Task Client_GuildAvailable(SocketGuild arg)
        {
            ISocketMessageChannel defaultChan = (ISocketMessageChannel)arg.TextChannels.FirstOrDefault<SocketTextChannel>();

            //Stuff for setting up guild
            _guildManager.CheckOrMakeGuild(arg);

            ConnectGuild connectGuild = _guildManager.GetGuild(arg);
            ISocketMessageChannel logChan = (ISocketMessageChannel)arg.GetChannel(connectGuild.config.logChannelID);

            ISocketMessageChannel target = logChan ?? defaultChan;

            if (connectGuild.config.botOnlineNotifaction)
            {
                EmbedBuilder embed = new EmbedBuilder();

                embed.WithTitle($"{_client.CurrentUser.Username} is back Online")
                    .WithDescription("Guess who. It's just me. Don't get your hopes up ._.")
                    .WithFooter(x =>
                    {
                        x.Text = $"{System.DateTime.UtcNow.TimeOfDay}";
                    });
                await target.SendMessageAsync("", false, embed.Build());
            }
        }

        public Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>(new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 50, AlwaysDownloadUsers = true }))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }

        //private async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        //{
        //    // Check if this was a non-bot user joining a voice channel
        //    if (user.IsBot)
        //        return;

        //    var guild = state2.VoiceChannel?.Guild ?? state1.VoiceChannel?.Guild;
        //    if (guild == null)
        //        return;

        //    //var result = from a in _connections.Keys
        //    //        where (a) == guild.Id
        //    //        select a;
        //    //var connections  = result.FirstOrDefault();
        //    var connection = _connections[guild.Id] ?? null;
        //    if (state2.VoiceChannel == null && state1.VoiceChannel != null && connection != null)
        //    {
        //        // Disconnected
        //        if (!state1.VoiceChannel.Users.Any(u => !u.IsBot))
        //        {
        //            Console.WriteLine("Disconnect a thing");
        //            await state1.VoiceChannel.DisconnectAsync();
        //        }
        //        return;
        //    }
            

        //    if (connection == null || connection.ConnectionState != ConnectionState.Connected)
        //    {
        //        ConnectToVoice(state2.VoiceChannel).Start();
        //    }
        //}

        //private async Task ConnectToVoice(SocketVoiceChannel voiceChannel)
        //{
        //    if (voiceChannel == null)
        //        return;

        //    try
        //    {
        //        Console.WriteLine($"Connecting to channel {voiceChannel.Id}");
        //        var connection = await voiceChannel.ConnectAsync();
        //        Console.WriteLine($"Connected to channel {voiceChannel.Id}");
        //        _connections[voiceChannel.Guild.Id] = connection;

        //        await Task.Delay(1000);
        //        //_lastZapped[voiceChannel.Guild.Id] = DateTime.Now;
        //        await Say(connection, "why.mp3");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Oh no, error
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine($"- {ex.StackTrace}");
        //    }
        //}
        

    }
}
