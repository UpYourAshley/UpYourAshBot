using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace UpYourAshBotReDo.Core.UserAccounts
{
    public class UserAccounts
    {
        public static List<UserAccount> accounts;

        public static string filePath;

        public UserAccounts(string _filePath)
        {
            LoggingUtils.LogInfo($"Loading accounts from path: {_filePath}");
            filePath = _filePath;

            if (DataStorage.SaveExists(filePath))
            {
                accounts = DataStorage.LoadUserAccounts(filePath).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public List<UserAccount> GetSortedAccountsAllTime()
        {
            List<UserAccount> sortedAccounts = accounts;
            UserAccount temp;
            for (int i = 1; i <= sortedAccounts.Count; i++)
            {
                for (int j = 0; j < sortedAccounts.Count - i; j++)
                {
                    if (sortedAccounts[j].XP < sortedAccounts[j + 1].XP)
                    {
                        temp = sortedAccounts[j];
                        sortedAccounts[j] = sortedAccounts[j + 1];
                        sortedAccounts[j + 1] = temp;
                    }
                }
            }

            return sortedAccounts;
        }

        public List<UserAccount> GetSortedAccountsWeek()
        {
            List<UserAccount> sortedAccounts = accounts;
            UserAccount temp;
            for (int i = 1; i <= sortedAccounts.Count; i++)
            {
                for (int j = 0; j < sortedAccounts.Count - i; j++)
                {
                    if (sortedAccounts[j].WeekXP < sortedAccounts[j + 1].WeekXP)
                    {
                        temp = sortedAccounts[j];
                        sortedAccounts[j] = sortedAccounts[j + 1];
                        sortedAccounts[j + 1] = temp;
                    }
                }
            }

            return sortedAccounts;
        }

        public void ClearWeeklyXP()
        {
            foreach (UserAccount account in accounts)
            {
                account.WeekXP = 0;
            }

            SaveAccounts();
        }

        public void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts, filePath);
        }

        public UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }


        private UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts
                         where a.ID == id
                         select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        private UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount()
            {
                ID = id,
                XP = 0,
                WeekXP = 0
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
