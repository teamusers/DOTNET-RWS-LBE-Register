using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.Acs.Responses
{
    public class AcsAuthResponseData
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
