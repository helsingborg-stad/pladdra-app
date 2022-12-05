using Newtonsoft.Json;

namespace Abilities.ARRoomAbility.WP.Schema
{
    public class WpScene
    {
        [JsonProperty("scene_name")]
        public string Name { get; set; }
//        [JsonProperty("scene_id")]
//        public string Id { get; set; }
        [JsonProperty("json")]
        public string Json { get; set; }
        [JsonProperty("scene_is_featured")]
        public bool IsFeatured { get; set; }
    }
}