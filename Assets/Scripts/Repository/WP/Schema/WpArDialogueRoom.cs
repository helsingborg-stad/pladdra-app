using System.Collections.Generic;
using Newtonsoft.Json;

namespace Repository.WP.Schema
{
    public class WpArDialogueRoom
    {
        public class AdvancedCustomFields
        {
            [JsonProperty("plane_dimensions_width")]
            public int PlaneDimensionsWidth { get; set; }
            [JsonProperty("plane_dimensions_height")]
            public int PlaneDimensionsHeight { get; set; }
            [JsonProperty("resources"), JsonConverter(typeof(ConvertFalseToNull))]
            public List<WpResource> Resources { get; set; }
            [JsonProperty("scenes"), JsonConverter(typeof(ConvertFalseToNull))]
            public List<WpScene> Scenes { get; set; }
        }
        [JsonProperty("acf")]
        public AdvancedCustomFields Acf { get; set; }
    }
}