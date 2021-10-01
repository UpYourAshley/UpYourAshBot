using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYourAshBotReDo
{
    internal static class Global
    {
        /// <summary>
        /// first is guild ID, second is message ID
        /// </summary>
        internal static Dictionary<ulong, ulong> watchMessages { get; set; } = new Dictionary<ulong, ulong>(); 
    }
}
