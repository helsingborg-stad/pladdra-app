using Newtonsoft.Json;

namespace Abilities.ARRoomAbility.WP.Schema
{
    public class WpScene
    {
        [JsonProperty("scene_name"), JsonConverter(typeof(ConvertFalseToNull))]
        public string Name { get; set; }
        [JsonProperty("json"), JsonConverter(typeof(ConvertFalseToNull))]
        public string Json { get; set; }
        [JsonProperty("scene_is_featured")]
        public bool IsFeatured { get; set; }
    }
}