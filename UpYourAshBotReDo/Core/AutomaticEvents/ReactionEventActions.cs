using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using UpYourAshBotReDo.Core.CustomRoles;

namespace UpYourAshBotReDo.Core.AutomaticEvents
{
    internal static class ReactionEventActions
    {
        //args.Count - 1 = the event (last element)
        //args.Count - 2 = SocketCommandContext (2nd to last element)
        //args.Count - 3 = List<Emoji> AddReactions (3rd last element)
        //args.Count - 4 = RestUserMessage message (4th last element)

        /// <summary>
        /// Removes a custom role
        /// </summary>
        /// <param name="vars">1st CustomRole, 2nd is bool to delete fully, 3rd is Context</param>
        public static async void RemoveCustomRole(List<dynamic> vars)
        {
            Console.WriteLine($"Vars length {vars.Count}");

            CustomRole customRole = vars[0];
            bool fullDelete = vars[1];
            RestUserMessage message = vars[vars.Count - 4];
            List<Emoji> reactionsTA = vars[vars.Count - 3];
            SocketCommandContext Context = vars[vars.Count -2];
            ReactionEvent rEvent = vars[vars.Count-1];
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            if(Context.User.Id != rEvent.userReactedID)
            {
                //await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"Removed role. Name: {customRole.Name}", Context));
                return;
            }

            if(rEvent.reactedWith.ToString() == "✅")
            {
                //Removes the custom role for the servers custom role list
                connectGuild.customRoles.customRoles.Remove(customRole);
                connectGuild.customRoles.SaveRoles();

                if (!fullDelete)
                {
                    await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Removed Custom Role", $"Removed role. Name: {customRole.Name}", Context));
                }
                else
                {
                    await Context.Guild.GetRole(customRole.ID).DeleteAsync();

                    await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Deleted Deleted Role", $"Deleted role. Name: {customRole.Name}", Context));
                }

                rEvent.beenTriggered = true;

            }
            else if(rEvent.reactedWith.ToString() == "❎")
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Canceled", $"Canceled the removal of the role. Name: {customRole.Name}", Context));
                rEvent.beenTriggered = true;
            }
            //For when its been reacted with by other reactions
            else
            {

            }
        }

        public static async void ClearWeeklyLeaderboard(List<dynamic> vars)
        {
            Console.WriteLine($"Vars length {vars.Count}");

            SocketCommandContext Context = vars[vars.Count-2];
            ReactionEvent rEvent = vars[vars.Count - 1];
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            if (Context.User.Id != rEvent.userReactedID)
            {
                //await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"Removed role. Name: {customRole.Name}", Context));
                return;
            }

            if (rEvent.reactedWith.ToString() == "✅")
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Cleared", $"The weekly scoreboard has been cleared.", Context));

                connectGuild.accounts.ClearWeeklyXP();

                rEvent.beenTriggered = true;

            }
            else if (rEvent.reactedWith.ToString() == "❎")
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Canceled", $"Canceled Clearing the weekly scoreboard.", Context));
                rEvent.beenTriggered = true;
            }
            //For when its been reacted with by other reactions
            else
            {

            }
        }

        public static void BanMember(List<dynamic> vars)
        {

        }

        public static void Clear(List<dynamic> vars)
        {

        }

        public static async void TimedOut(SocketCommandContext Context)
        {
            await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Timed Out", $"Action Timed Out", Context));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vars">1st = question, 2nd = 1st reaction, 3rd =2nd reaction, 4th = Context, 5th = ReactionEvent</param>
        public static async void Ask(List<dynamic> vars)
        {
            string message = vars[0];
            dynamic option1 = vars[1];
            dynamic option2 = vars[2];
            SocketCommandContext context = vars[3];
            ReactionEvent rEvent = vars[4];

            if(rEvent.reactedWith.ToString() == option1)
            {
                await context.Channel.SendMessageAsync($"Nice to see you are doign good, {context.User.Username}.");
            }
            else
            {
                await context.Channel.SendMessageAsync($"Thats an oof, {context.User.Username}.");
            }

            

        }

        public static async void SetUp(RestUserMessage message, List<Emoji> reactions, SocketCommandContext Context)
        {
            foreach(Emoji emoji in reactions)
            {
                await message.AddReactionAsync(emoji);
            }

            Global.watchMessages.Add(message.Id, Context.Guild.Id);
        }
        public static void Test()
        {
            CustomRole e = new CustomRole
            {
                ID = 1,
                Name = "Yeet"
            };
            string a = "yes";
            List<dynamic> dynamicList = new List<dynamic>();
            dynamicList.Add(e);
            dynamicList.Add(a);

            Console.WriteLine($"List Made, it has {dynamicList.Count} items inside.");
        }
    }
}
