using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class IdentifierDto
    {
        [JsonPropertyName("external_id")]
        public string? ExternalID { get; set; }

        [JsonPropertyName("external_id_type")]
        public string? ExternalIDType { get; set; }
    }
}
