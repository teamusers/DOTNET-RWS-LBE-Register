using System.Text.Json.Serialization;
using RWS_LBE_Register.Helpers;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class CarDetailDto
    {
        [JsonPropertyName("car_plate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CarPlate { get; set; }

        [JsonPropertyName("iu_number")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IuNumber { get; set; }

        [JsonPropertyName("issg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Issg { get; set; }

        [JsonPropertyName("last_updated")]
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? LastUpdated { get; set; }
    }
}
