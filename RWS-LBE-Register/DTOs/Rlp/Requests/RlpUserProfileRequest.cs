using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.Rlp.Requests
{
    public class UserTierUpdateEventRequest
    {
        [JsonPropertyName("event_lookup")]
        public string? EventLookup { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("retailer_id")]
        public string? RetailerID { get; set; }
    }

    public class UserProfileRequest
    {
        [JsonPropertyName("user")]
        public RlpUserRequest User { get; set; } = new();
    }
}
