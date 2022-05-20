using System;
using Newtonsoft.Json;

namespace Repository.WP.Schema
{
    public class ConvertFalseToNull : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Boolean)
            {
                if ((bool)reader.Value == false)
                    return null;
            }

            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}