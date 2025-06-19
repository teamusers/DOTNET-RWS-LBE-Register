using System.Text.Json.Serialization;
using RWS_LBE_Register.DTOs.User.Shared;

namespace RWS_LBE_Register.DTOs.Responses
{
    public class GetUserProfileResponse
    {
        [JsonPropertyName("user")]
        public UserDto User { get; set; } = new();
    }

    public class GetUserIdExtensionsResponseData
    {
        [JsonPropertyName("rlp_id")]
        public string? RlpId { get; set; }
    }
}
