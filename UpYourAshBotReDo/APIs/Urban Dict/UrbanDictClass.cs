using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYourAshBotReDo.APIs.Urban_Dict
{
    public class UrbanDictClass
    {
        [JsonProperty("list")]
        public IList<List> list { get; set; }
    }

    public class List
    {
        [JsonProperty("definition")]
        public string definition { get; set; }

        [JsonProperty("permalink")]
        public string permalink { get; set; }

        [JsonProperty("thumbs_up")]
        public int thumbs_up { get; set; }

        [JsonProperty("sound_urls")]
        public IList<string> sound_urls { get; set; }

        [JsonProperty("author")]
        public string author { get; set; }

        [JsonProperty("word")]
        public string word { get; set; }

        [JsonProperty("defid")]
        public int defid { get; set; }

        [JsonProperty("current_vote")]
        public string current_vote { get; set; }

        [JsonProperty("written_on")]
        public DateTime written_on { get; set; }

        [JsonProperty("example")]
        public string example { get; set; }

        [JsonProperty("thumbs_down")]
        public int thumbs_down { get; set; }
    }
}
