using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYourAshBotReDo
{
    public class GuildConfig
    {
        public string name;
        public ulong id;
        public string filePath;
        public string userAccountPath;
        public string customRolePath;
        public string workOptionsPath;
        public string eightBallResultsPath;
        public string eventsPath;

        public bool botOnlineNotifaction = false;

        public bool levelUpMessage = false;

        public bool userJoinedNotifaction = true;
        public bool showJoinMessage;
        public string joinMessage;

        public bool userLeftNotifaction = true;
        public string userLeftMessage;

        public int workCoolDown;
        public int robCoolDown;

        public ulong joinLeftChannelID;
        public ulong logChannelID;
        public ulong announcementChannelID;
        public ulong eventAnnouncementChannelID;

        public bool userVerification;
        public ulong userVerificationRoleID;
        public ulong userVerificationMessageID;
    }
}
