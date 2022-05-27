using Newtonsoft.Json;

namespace Abilities.ARRoomAbility.WP.Schema
{
    public class WpMarker
    {
        [JsonProperty("image")]
        public string Image { get; set; }        
        [JsonProperty("width")]
        public float Width { get; set; }
        [JsonProperty("height")]
        public float Height { get; set; }
    }
}