using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using UpYourAshBotReDo.Core.AutomaticEvents;
using UpYourAshBotReDo.Core.CustomRoles;

namespace UpYourAshBotReDo.Modules
{
    public class CustomRoles : ModuleBase<SocketCommandContext>
    {
        [Command("CustomRoles")]
        public async Task ListCustomRoles()
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);


            await Context.Channel.SendMessageAsync($"There are currently {connectGuild.customRoles.customRoles.Count()} custom Roles.");
        }


        [Command("ListCustomRoles"), Alias("LCR")]
        public async Task ListCustom([Remainder] string page = "1")
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);


            string pageNum = page;
            int startIndex = (Convert.ToInt32(pageNum) - 1) * 10;
            int endIndex = startIndex + 10;
            EmbedBuilder embed = new EmbedBuilder();

            //int numPages = connectGuild.customRoles.customRoles.Count / 10;
            int numPages = (int)Math.Ceiling((double)connectGuild.customRoles.customRoles.Count / 10);
            if (numPages < 1) numPages = 1;

            if(Convert.ToInt32(page) > numPages)
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"There is no page {page}.", Context));
                return;
            }

            //max items per page = 10
            for (int i = startIndex; i != endIndex; i++)
            {
                if (connectGuild.customRoles.customRoles.Count > i)
                {
                    CustomRole game = connectGuild.customRoles.customRoles[i];
                    embed.AddField($"ID: {connectGuild.customRoles.customRoles[i].ID}", $"**{game.Name}**");
                }
            }


            embed.WithTitle("Custom Roles")
                .WithDescription("Do **|IAm <roleName>** to assign yourself to a role and **IAmNot <roleName>** to unassign yourself. - This is still in development :)")
                .AddField("Page Num - ", $"{pageNum}/{numPages}");

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }


        /// <summary>
        /// Adds a new custom role
        /// </summary>
        /// <param name="vars">The input data split by '|' in order of  Name, Colour</param>
        /// <returns></returns>
        [Command("AddCustomRole"), Alias("ACR")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddCustomRole([Remainder] string vars)
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            CustomRole newRole = new CustomRole();

            string[] splitVars = vars.Split('|');

            //Using preexisting roles ID
            if (splitVars.Length == 1)
            {
                IRole role;

                ulong roleID = 0;
                ulong.TryParse(splitVars[0], out roleID);

                if (roleID != 0)
                {
                    role = Context.Guild.GetRole(roleID);
                }
                else
                {
                    var result = from a in Context.Guild.Roles
                                 where a.Name.ToLower() == splitVars[0].ToLower()
                                 select a;

                    role = result.FirstOrDefault();
                }
                
                if(role == null)
                {
                    await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"Couldnt find a role with ID/Name: {splitVars[0]}. Sorry.", Context));
                    return;
                }

                newRole.ID = role.Id;
                newRole.Name = role.Name;

                connectGuild.customRoles.customRoles.Add(newRole);
                connectGuild.customRoles.SaveRoles();

                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Added Custom Role", $"The **{role.Name}** role is now classed as a custom role.", Context));

                return;
            }
            //checking if vars for new role are there
            else if (splitVars.Length < 4)
            {
                await Context.Channel.SendMessageAsync("Please specify the name and colour of the role - Split by '|'.");
                return;
            }
            //new role
            else 
            {
                // Color c = new Color(Convert.ToUInt32(splitVars[1]));
                Color c = new Color(Convert.ToInt32(splitVars[1]), Convert.ToInt32(splitVars[2]), Convert.ToInt32(splitVars[3]));
                //System.Drawing.Color c = ColorTranslator.FromHtml(splitVars[1]);

                IRole role = await Context.Guild.CreateRoleAsync(splitVars[0], null, c);

                newRole.ID = role.Id;
                newRole.Name = role.Name;

                connectGuild.customRoles.customRoles.Add(newRole);
                connectGuild.customRoles.SaveRoles();

                await Context.Channel.SendMessageAsync($"Created new role: {role.Mention}");
            }

            
            

            await Context.Channel.SendMessageAsync("Coming Soon :)");
        }

        

        [Command("RemoveCustomRole"), Alias("RCR")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveCustomRole([Remainder] string vars)
        {

            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            string[] splitVars = vars.Split('|');
            bool hardDelete = false;
            //if there are 2 or more inputted vars 
            if(splitVars.Length >= 2)
            {
                //sets hard deleet to the inputed value
                Console.WriteLine(splitVars[1]);
                hardDelete = Convert.ToBoolean(splitVars[1]);
            }

            CustomRole cR = connectGuild.customRoles.GetRole(splitVars[0]);
            
            if (cR == null)
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"Couldnt find a role with ID/Name: {splitVars[0]}. Sorry.", Context));
                return;
            }

            #region setting up the reaction event
            
            RestUserMessage lastMSG = await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Are you sure?", $"Are you sure you want to remove **{cR.Name}**?", Context));

            List<Emoji> emojis = new List<Emoji>();
            emojis.Add(new Emoji("✅"));
            emojis.Add(new Emoji("❎"));

            List<dynamic> param = new List<dynamic>();
            param.Add(cR);
            param.Add(hardDelete);
            param.Add(lastMSG);
            param.Add(emojis);
            param.Add(Context);


            ReactionEventActions.SetUp(lastMSG, emojis, Context);

            //TODO - have it add reactions and set to watched messages automatically some how
            //await lastMSG.AddReactionAsync(new Emoji("✅"));
            //await lastMSG.AddReactionAsync(new Emoji("❎"));
            //Global.watchMessages.Add(Context.Guild.Id, lastMSG.Id);
            //Global.watchMessages.Add(lastMSG.Id, Context.Guild.Id);

            Console.WriteLine($"Parms count {param.Count}");

            ReactionEvent newEvent = new ReactionEvent()
            {
                messageID = lastMSG.Id,
                startTime = DateTime.Now,
                args = param,
                onTriggeredCall = ReactionEventActions.RemoveCustomRole,
                timeOut = 10
            };

            #endregion
        }

        [Command("IAm")]
        public async Task IAm([Remainder]string name)
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            CustomRole role = connectGuild.customRoles.GetRole(name);

            if (role == null)
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"Could not find role with the Name: {name}.", Context));
                return;
            }

            SocketGuildUser user = (SocketGuildUser)Context.User;

            if (user.Roles.Contains(Context.Guild.GetRole(role.ID)))
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"You alreay have the role: {name}.", Context));
                return;
            }

            await user.AddRoleAsync(Context.Guild.GetRole(role.ID));
            await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Added Role", $"You now have the **{name}** role.", Context));
        }

        [Command("IAmNot")]
        public async Task IAmNot([Remainder]string name)
        {
            ConnectGuild connectGuild = Program._guildManager.GetGuild(Context.Guild);

            CustomRole role = connectGuild.customRoles.GetRole(name);

            if (role == null)
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"Could not find role with the Name: {name}.", Context));
                return;
            }

            SocketGuildUser user = (SocketGuildUser)Context.User;

            if (!user.Roles.Contains(Context.Guild.GetRole(role.ID)))
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Error", $"You do not have the role: {name}.", Context));
                return;
            }

            await user.RemoveRoleAsync(Context.Guild.GetRole(role.ID));
            await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Removed Role", $"You no longer have the **{name}** role.", Context));
        }
    }
}


///Scrap all this shit.
///Redo it in a way that makes sence 
///and isn't ass for the end user
///and actually isnt confusing af
///also redo the way it tracks reactions on messages 
///and how events related to reactions to certain messages 
///are handled and trriggered
///and dont be a dumb ass XD 
///oh and try not make it have to use multithreading 
///cos fuck having to mess with that XD.
