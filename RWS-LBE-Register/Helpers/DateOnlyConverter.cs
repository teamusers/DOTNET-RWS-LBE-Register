using System.Text.Json;
using System.Text.Json.Serialization;

namespace RWS_LBE_Register.Helpers
{
    public class DateOnlyConverter : JsonConverter<DateTime?>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String &&
                DateTime.TryParseExact(reader.GetString(), Format, null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString(Format));
            else
                writer.WriteNullValue();
        }

        public DateOnlyConverter() { }
    }
}
