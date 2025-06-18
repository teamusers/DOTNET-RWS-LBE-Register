using System;
using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class CarDetailDto
    {
        [JsonPropertyName("car_plate")]
        public string? CarPlate { get; set; }

        [JsonPropertyName("iu_number")]
        public string? IuNumber { get; set; }

        [JsonPropertyName("issg")]
        public bool? Issg { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime? LastUpdated { get; set; }
    }
}
