using Newtonsoft.Json;

namespace Abilities.ARRoomAbility.WP.Schema
{
    public class WpResource 
    {
        [JsonProperty("name"), JsonConverter(typeof(ConvertFalseToNull))]
        public string Name { get; set; }
        [JsonProperty("model"), JsonConverter(typeof(ConvertFalseToNull))]
        public string Model { get; set; }
        [JsonProperty("marker_model"), JsonConverter(typeof(ConvertFalseToNull))]
        public string MarkerModel { get; set; }
        [JsonProperty("disable")]
        public bool Disable { get; set; }
    }
}