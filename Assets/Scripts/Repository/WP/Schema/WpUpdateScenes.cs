using System.Collections.Generic;
using Newtonsoft.Json;

namespace Repository.WP.Schema
{
    public class WpUpdateScenes
    {
        public class AdvancedCustomFields
        {
            [JsonProperty("scenes")]
            public List<WpScene> Scenes { get; set; }
        }
        [JsonProperty("acf")]
        public AdvancedCustomFields Acf { get; set; }
    }
}