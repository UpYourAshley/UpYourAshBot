using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using UpYourAshBotReDo.Core.LevelingSystem;
using UpYourAshBotReDo.Core.UserAccounts;

namespace UpYourAshBotReDo.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            //Hook commandexecuted to handle post command logic
            _commands.CommandExecuted += CommandExecutedAsync;

            //Hook messagerecieved so we can process messages to see command
            _discord.MessageReceived += MessageReceivedAsync;

        }

        public async Task InitializeAsync()
        {
            //Register modules 
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {

            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            // This value holds the offset where the prefix ends
            var argPos = 0;
            // Perform prefix check. You may want to replace this with
            // (!message.HasCharPrefix('!', ref argPos))
            // for a more traditional command format like !help.
            //if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)
            //    && !message.HasStringPrefix(Config.bot.cmdPrefix, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            if (context.Guild == null)
            {
                Random r = new Random();
                await context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed($"Automated Response - Task {r.Next(0, 10000)}", "Thank you for your message, we will get back to you **never**. Signed, Ash the BOT", context));
                return;
            }

            #region CUSTOM
            //Mute Check
            UserAccounts userAccounts = Program._guildManager.GetGuild(context.Guild).accounts;
            var userAccount = userAccounts.GetAccount((SocketUser)context.User);
            if (userAccount.IsMuted)
            {
                await context.Message.DeleteAsync();
                return;
            }
            
            //LEVELING
            if (!context.Message.HasStringPrefix(Config.bot.cmdPrefix, ref argPos)
               && !message.HasMentionPrefix(_discord.CurrentUser, ref argPos))
                Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel, (SocketGuild)context.Guild);

            //string[] bannedWORDS = new string[] { "white", "wh1te", "w h i t e", "wh i t e", "w hi t e", "wh ite"};
            ////BANNED 
            //if (bannedWORDS.Contains(context.Message.ToString().ToLower()))
            //{
            //    await context.Message.DeleteAsync();
            //}
            //CONFIG VARS


            //hello there
            if (context.Message.ToString().ToLower() == "hello there")
            {
                await context.Channel.SendMessageAsync("General Kenobi");
            }
            #endregion

            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            var msg = rawMessage as SocketUserMessage;
            if (msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                await _commands.ExecuteAsync(context, argPos, _services);
            }
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
            {
                return;
            }

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }

    }
}
