using System.Text.Json.Serialization;
namespace RWS_LBE_Register.DTOs.Rlp.Responses
{
    public static class RlpErrorCodes
    {
        public const string UserNotFound = "user_not_found";
    }

    public class GetUserResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("user")]
        public RlpUserResp User { get; set; } = new();
    }

    public class UserProfileErrorResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("errors")]
        public RlpErrorDetail Errors { get; set; } = new();
    }

    public class RlpErrorDetail
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("user")]
        public object? User { get; set; } // Matches Go's `any` type
    }
}
