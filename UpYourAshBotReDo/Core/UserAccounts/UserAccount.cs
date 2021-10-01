using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYourAshBotReDo.Core.UserAccounts
{
    public class UserAccount
    {
        public string Name { get; set; }

        public ulong ID { get; set; }

        public uint XP { get; set; }

        public uint WeekXP { get; set; }

        public DateTime LastMessage { get; set; }

        public uint LevelNumber
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }

        public bool IsMuted { get; set; }
    }
}
