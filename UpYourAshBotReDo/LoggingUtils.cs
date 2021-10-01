using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYourAshBotReDo
{
    static class LoggingUtils
    {
        public static void LogInfo(string info)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Info : {DateTime.Now.ToShortTimeString()} : {info}");
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void LogError(string info)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Info : {DateTime.Now.ToShortTimeString()} : {info}");
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void LogProcess(string info)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Info : {DateTime.Now.ToShortTimeString()} : {info}");
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
