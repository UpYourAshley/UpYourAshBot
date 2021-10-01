using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYourAshBotReDo.Core.AutomaticEvents
{
    public class ReactionEvent
    {
        public static List<ReactionEvent> reactionEvents = new List<ReactionEvent>();

        public ulong messageID;

        public DateTime startTime;
        public int timeOut;

        public bool beenTriggered = false;

        public Emoji reactedWith;
        public ulong userReactedID;

        public delegate void OnTrigger(List<dynamic> vars);

        public OnTrigger onTriggeredCall;

        public List<dynamic> args = new List<dynamic>();

        public ReactionEvent()
        {
            reactionEvents.Add(this);
        }

        public void Trigger()
        {
            args.Add(this);
            onTriggeredCall(args);
        }

        public void TimeOut()
        {
            beenTriggered = true;
            ReactionEventActions.TimedOut(args[args.Count - 1]);
        }
    }
}
