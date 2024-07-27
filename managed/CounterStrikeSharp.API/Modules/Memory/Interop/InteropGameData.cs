using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CounterStrikeSharp.API.Modules.Memory.Interop
{
    public class InteropGameData
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static Dictionary<string, PlatformData> ReadFrom(string configName)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, PlatformData>>(File.ReadAllText(Path.Combine(Application.Instance.PluginGameDataDirectory, $"{configName}.json")), Options) ?? [];
            }
            catch (Exception ex)
            {
                Application.Instance.Logger.LogError($"{ex.Message}\n{ex.StackTrace}");
            }

            return [];
        }
    }

#pragma warning disable 8600, 8602, 8603, 8618 // Visual Studio真是有够烦的
    public class PlatformData
    {
        [JsonConverter(typeof(JsonStringOrIntConverter))]
        [JsonPropertyName("windows")]
        public object Windows { get; set; }

        [JsonConverter(typeof(JsonStringOrIntConverter))]
        [JsonPropertyName("linux")]
        public object Linux { get; set; }
    }

    public class JsonStringOrIntConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return reader.GetString();
            else if (reader.TokenType == JsonTokenType.Number)
                return reader.GetInt32();
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is string)
                writer.WriteStringValue((string)value);
            else if (value is int)
                writer.WriteNumberValue((int)value);
            else
                throw new JsonException();
        }
    }
#pragma warning restore
}
