using Newtonsoft.Json;

namespace Abilities.ARRoomAbility.WP.Schema
{
    public class WpResource 
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("marker_model")]
        public string MarkerModel { get; set; }
        [JsonProperty("disable")]
        public bool Disable { get; set; }
    }
}