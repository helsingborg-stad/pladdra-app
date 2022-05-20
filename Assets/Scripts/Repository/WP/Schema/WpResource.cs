using Newtonsoft.Json;

namespace Repository.WP.Schema
{
    public class WpResource 
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("marker_model")]
        public string MarkerModel { get; set; }
    }
}