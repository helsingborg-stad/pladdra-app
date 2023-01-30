using System.Collections.Generic;
using Newtonsoft.Json;

namespace Abilities.ARRoomAbility.WP.Schema
{
    public class WpArDialogueRoomUpdate
    {
        public class AdvancedCustomFields
        {
            [JsonProperty("scenes"), JsonConverter(typeof(ConvertFalseToNull))]
            public List<WpScene> Scenes { get; set; }
        }
        [JsonProperty("acf")]
        public AdvancedCustomFields Acf { get; set; }
    }
}