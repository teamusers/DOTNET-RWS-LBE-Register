using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class PhoneNumberDto
    {
        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("phone_type")]
        public string? PhoneType { get; set; }

        [JsonPropertyName("preference_flags")]
        public List<string> PreferenceFlags { get; set; } = new();

        [JsonPropertyName("verified_ownership")]
        public bool VerifiedOwnership { get; set; }
    }
}
