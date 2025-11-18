using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace REAC.Domain.Shared.Helpers
{
    public class StringOrIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    return reader.GetInt32();

                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (int.TryParse(stringValue, out int result))
                    {
                        return result;
                    }
                    return 0; // or throw exception

                default:
                    throw new JsonException($"Cannot convert {reader.TokenType} to int");
            }
        }

   

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }

       
    }
}
