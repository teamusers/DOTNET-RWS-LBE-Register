using System.Text.Json;
using System.Text.Json.Serialization;

namespace RWS_LBE_Register.Helpers
{
    public class DateTimeConverter : JsonConverter<DateTime?>
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (DateTime.TryParseExact(str, Format, null, System.Globalization.DateTimeStyles.None, out var dt))
                    return dt;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString(Format));
        }

        public DateTimeConverter() { }
    }
}
