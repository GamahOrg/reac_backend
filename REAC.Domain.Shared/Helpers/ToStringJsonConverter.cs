using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Domain.Shared.Helpers
{
    public class ToStringJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(long);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                // Attempt to parse the string to a long
                if (long.TryParse(reader.Value.ToString(), out long value))
                {
                    return value;
                }
                throw new JsonException("Unable to convert string to long.");
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                return reader.Value;
            }

            throw new JsonException("Expected string or integer token.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
