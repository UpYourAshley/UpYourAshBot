using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Discord;
using Discord.Commands;

namespace UpYourAshBotReDo
{
    class Utilities
    {
        private static Random r = new Random();

        public static Dictionary<string, string> alerts;

        static Utilities()
        {
            try
            {
                string json = File.ReadAllText("SystemLang/alerts.json");
                //converts to data struct
                var data = JsonConvert.DeserializeObject<dynamic>(json);
                //converts to dict
                alerts = data.ToObject<Dictionary<string, string>>();
            }
            catch (Exception e)
            {

            }
        }

        public static string GetAlert(string key)
        {
            //sees if alert is tehre and if so it returns it
            if (alerts.ContainsKey(key))
            {
                return alerts[key];
            }

            return "";
        }

        public static string GetFormattedAlert(string key, object[] parameter)
        {
            if (alerts.ContainsKey(key))
            {
                return String.Format(alerts[key], parameter);
            }

            return "";
        }

        public static string GetFormattedAlert(string key, object parameter)
        {
            return GetFormattedAlert(key, new object[] { parameter });
        }

        public static Color GetColor()
        {
            return new Color(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
        }

        public static Embed EasyEmbed(string title, string description, SocketCommandContext context)
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle(title)
                .WithDescription(description)
                .WithFooter(x =>
                {
                    x.Text = $"{context.User.Username}#{context.User.Discriminator} at {context.Message.Timestamp}";
                    x.IconUrl = context.User.GetAvatarUrl();
                })
                .WithColor(GetColor());

            return embed.Build();
        }

        public static Embed EasyEmbed(string title, string description, string inlineTitle, string inlineValue, SocketCommandContext context)
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle(title)
                .WithDescription(description)
                 .WithFooter(x =>
                 {
                     x.Text = $"{context.User.Username}#{context.User.Discriminator} at {context.Message.Timestamp}";
                     x.IconUrl = context.User.GetAvatarUrl();
                 })
                 .WithColor(GetColor())
                 .AddField(inlineTitle, inlineValue);

            return embed.Build();
        }

    }
}
