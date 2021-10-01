using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UpYourAshBotReDo.APIs.Urban_Dict;

namespace UpYourAshBotReDo.APIs
{
    public static class APIManager
    {
        public static async Task<UrbanDictClass> GetDef(string word)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.GetAsync($"http://api.urbandictionary.com/v0/define?term={word}");

                string asString = await result.Content.ReadAsStringAsync();
                //Console.WriteLine($"Response = {asString}");
                //Console.WriteLine("-------------------------");

                UrbanDictClass response = JsonConvert.DeserializeObject<UrbanDictClass>(asString);


                return response ?? null;
            }
        }
    }
}
