using System.Text.Json.Serialization;
using RWS_LBE_Register.DTOs.User.Shared;
using RWS_LBE_Register.Helpers;

namespace RWS_LBE_Register.DTOs.User.Responses
{
    public class VerifyGrUserResponseData
    {
        [JsonPropertyName("user")]
        public UserDto User { get; set; } = new();

        // Otp fields assumed flattened into this class — from a shared Otp DTO
        [JsonPropertyName("otp_code")]
        public string OtpCode { get; set; } = string.Empty;

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }

    public class VerifyGrCmsUserResponseData
    {
        [JsonPropertyName("reg_id")]
        public string RegId { get; set; } = string.Empty;

        [JsonPropertyName("dob")]
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly Dob { get; set; }
    }

    public class CreateUserResponseData
    {
        [JsonPropertyName("user")]
        public UserDto User { get; set; } = new();
    }
}
