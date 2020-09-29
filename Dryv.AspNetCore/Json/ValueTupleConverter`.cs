using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dryv.AspNetCore.Json
{
    internal class ValueTupleConverter<T1> : JsonConverter<ValueTuple<T1>>
    {
        public override ValueTuple<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ValueTuple<T1> result = default;

            if (!reader.Read())
            {
                throw new JsonException();
            }

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.ValueTextEquals(options.PropertyNamingPolicy.ConvertName("Item1")) && reader.Read())
                {
                    result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                }
                else
                {
                    throw new JsonException();
                }

                reader.Read();
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ValueTuple<T1> value, JsonSerializerOptions options)
        {   
            writer.WriteStartObject();
            writer.WritePropertyName(options.PropertyNamingPolicy.ConvertName("Item1"));
            JsonSerializer.Serialize(writer, value.Item1, options);
            writer.WriteEndObject();
        }
    }
}