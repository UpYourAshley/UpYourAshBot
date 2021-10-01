using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Newtonsoft.Json;
using UpYourAshBotReDo.Core.AutomaticEvents;
namespace UpYourAshBotReDo.Modules
{
    public class GoogleCalandarCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ListBotCalandar")]
        public async Task ListBotEvents()
        {
            EmbedBuilder embed = new EmbedBuilder();

            EventsResource.ListRequest request = Program.calenderService.Events.List("v49g6l9md9b4bv5j0ja2sftk8c@group.calendar.google.com");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = request.Execute();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                    embed.AddField("Event", $"{eventItem.Summary} ({when})");
                }

                embed.WithTitle("UpComing Bot Events")
                .WithDescription("Upcoming events in the bot event google calandar - This is still in development :)")
                .WithFooter(x =>
                {
                    x.Text = $"{Context.User.Username}#{Context.User.Discriminator} at {Context.Message.Timestamp}";
                    x.IconUrl = Context.User.GetAvatarUrl();
                })
                 .WithColor(Utilities.GetColor());

                await Context.Channel.SendMessageAsync("", embed: embed.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Resposne", "No upcoming bot events found.", Context));
                Console.WriteLine("No upcoming events found.");
            }
        }

        [Command("TestAdd")]
        public async Task AddCalandarEvent()
        {
            Event newEvent = new Event()
            {
                Summary = "Test Event TBH",
                Location = "The internet",
                Description = "Just testing shit"
            };

            DateTime startDateTime = new DateTime(2020, 5, 20, 12, 30, 00);
            EventDateTime start = new EventDateTime()
            {
                DateTime = startDateTime,
                TimeZone = "Etc/UTC"
            };
            newEvent.Start = start;

            DateTime endDateTime = new DateTime(2020, 5, 21, 12, 30, 00);
            EventDateTime end = new EventDateTime()
            {
                DateTime = endDateTime,
                TimeZone = "Etc/UTC"
            };
            newEvent.End = end;

            newEvent = await Program.calenderService.Events.Insert(newEvent, "t33776qhe8olek31ghtjbf46e4@group.calendar.google.com").ExecuteAsync();
        }


        //name is auto as the person who is getting trained + type 
        //then @student, @teacher(s), Date, Time
        [Command("AddGlobalEvent")]
        public async Task AddGlobalEvent(string startDate, string startTime, string endDate, string endTime, [Remainder] string vars)
        {
            //day/month/year - If time not given defaults to 12
            DateTime startDT = Convert.ToDateTime($"{startDate} {startTime}");
            DateTime endDT = Convert.ToDateTime($"{endDate} {endTime}");

            //0 Title, 1 Description, 3 location
            string[] splitVars = vars.Split('|');


            Event newEvent = new Event()
            {
                Summary = splitVars[0],
                Location = "The internet",
                Description = splitVars[1]
            };

            EventDateTime start = new EventDateTime()
            {
                DateTime = startDT,
                TimeZone = "Etc/UTC"
            };
            newEvent.Start = start;

            EventDateTime end = new EventDateTime()
            {
                DateTime = endDT,
                TimeZone = "Etc/UTC"
            };
            newEvent.End = end;

            newEvent = await Program.calenderService.Events.Insert(newEvent, "t33776qhe8olek31ghtjbf46e4@group.calendar.google.com").ExecuteAsync();

            await Context.Channel.SendMessageAsync("", embed: Utilities.EasyEmbed("Event Created", $"Link: {newEvent.HtmlLink}", Context));
        }
    }
}
