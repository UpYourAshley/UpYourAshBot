using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using UpYourAshBotReDo.Core.UserAccounts;
using UpYourAshBotReDo.Core.LevelingSystem;
using UpYourAshBotReDo.Core.CustomRoles;

namespace UpYourAshBotReDo.Core
{
    public static class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static void SaveCustomRoles(IEnumerable<CustomRole> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<CustomRole> LoadCustomRoles(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<CustomRole>>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool DirExists(string filePath)
        {
            return Directory.Exists(filePath);
        }
    }
}
