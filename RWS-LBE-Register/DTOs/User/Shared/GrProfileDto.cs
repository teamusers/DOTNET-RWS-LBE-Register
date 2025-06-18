using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class GrProfileDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("pin")]
        public string? Pin { get; set; }

        [JsonPropertyName("class")]
        public string? Class { get; set; }
    }
}
