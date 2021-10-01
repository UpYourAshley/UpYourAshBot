using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using UpYourAshBotReDo.Core.AutomaticEvents;

namespace UpYourAshBotReDo.Core.Time
{
    internal static class RepeatingTimer
    {
        public static Timer loopingTimer;

        public static DateTime resetTimeMin = new DateTime(2020, 5, 7, 12, 00, 00);
        public static DateTime resetTimeMax = new DateTime(2020, 5, 7, 12, 01, 00);

        internal static Task StartTimer()
        {
            Console.WriteLine("Started Timer");

            loopingTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true
            };
            loopingTimer.Elapsed += OnTimerTicked;

            return Task.CompletedTask;
        }

        public static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {

            //if between 12:00 & 12:01 it resets
            DateTime now = System.DateTime.Now;
            if (now.DayOfWeek == DayOfWeek.Monday)
            {
                //1 is later than , 0 is same, -1 is earlier
                int pastMidRes = now.TimeOfDay.CompareTo(resetTimeMin.TimeOfDay);
                int pastMaxRes = now.TimeOfDay.CompareTo(resetTimeMax.TimeOfDay);
                if((pastMidRes >0) && (pastMaxRes < 0))
                {
                    //RESET
                }
            }

            List<ReactionEvent> validEvents = new List<ReactionEvent>();

            //TODO Check reactionEvents and remove ones that have timed out.
            foreach(ReactionEvent rEvent in ReactionEvent.reactionEvents)
            {
                //if (rEvent.beenTriggered)
                //{
                //    Console.WriteLine("removing event");
                //    return;
                //}
                //else
                //{
                //    TimeSpan timeSpan = DateTime.UtcNow - rEvent.startTime.ToUniversalTime();
                //    Console.WriteLine(timeSpan);
                //    if (timeSpan > TimeSpan.FromSeconds(rEvent.timeOut))
                //    {
                //        Console.WriteLine("Event Timed Out");
                //        rEvent.TimeOut();
                //    }
                //}

                TimeSpan timeSpan = DateTime.UtcNow - rEvent.startTime.ToUniversalTime();
                Console.WriteLine(timeSpan);
                if (timeSpan > TimeSpan.FromSeconds(rEvent.timeOut))
                {
                    Console.WriteLine("Event Timed Out");
                    rEvent.TimeOut();
                }

                if (!rEvent.beenTriggered)
                {
                    validEvents.Add(rEvent);
                }
                else
                {
                    Console.WriteLine("Invalid Event");
                }

            }

            ReactionEvent.reactionEvents = validEvents;

        }

        
    }
}
