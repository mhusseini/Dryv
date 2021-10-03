using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dryv.AspNetCore.Json
{
    internal class ValueTupleConverter<T1, T2, T3> : JsonConverter<(T1, T2, T3)>
    {
        public override (T1, T2, T3) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            (T1, T2, T3) result = default;

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
                else if (reader.ValueTextEquals(options.PropertyNamingPolicy.ConvertName("Item2")) && reader.Read())
                {
                    result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
                }
                else if (reader.ValueTextEquals(options.PropertyNamingPolicy.ConvertName("Item3")) && reader.Read())
                {
                    result.Item3 = JsonSerializer.Deserialize<T3>(ref reader, options);
                }
                else
                {
                    throw new JsonException();
                }
                
                reader.Read();
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, (T1, T2, T3) value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(options.PropertyNamingPolicy.ConvertName("Item1"));
            JsonSerializer.Serialize(writer, value.Item1, options);
            writer.WritePropertyName(options.PropertyNamingPolicy.ConvertName("Item2"));
            JsonSerializer.Serialize(writer, value.Item2, options);
            writer.WritePropertyName(options.PropertyNamingPolicy.ConvertName("Item3"));
            JsonSerializer.Serialize(writer, value.Item3, options);
            writer.WriteEndObject();
        }
    }
}