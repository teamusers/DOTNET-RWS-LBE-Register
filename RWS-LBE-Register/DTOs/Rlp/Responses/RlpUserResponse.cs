using System.Text.Json.Serialization;
using RWS_LBE_Register.DTOs.User.Shared;
using RWS_LBE_Register.Helpers;

namespace RWS_LBE_Register.DTOs.Rlp.Responses
{
    public class RlpUserResp
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("external_id")]
        public string ExternalID { get; set; } = string.Empty;

        [JsonPropertyName("proxy_ids")]
        public List<string> ProxyIDs { get; set; } = new();

        [JsonPropertyName("opted_in")]
        public bool OptedIn { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("identifiers")]
        public List<IdentifierDto> Identifiers { get; set; } = new();

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;

        [JsonPropertyName("dob")]
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateTime? Dob { get; set; }

        [JsonPropertyName("account_status")]
        public string AccountStatus { get; set; } = string.Empty;

        [JsonPropertyName("auth_token")]
        public string AuthToken { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("address2")]
        public string Address2 { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("zip")]
        public string Zip { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("available_points")]
        public double AvailablePoints { get; set; }

        [JsonPropertyName("tier")]
        public string Tier { get; set; } = string.Empty;

        [JsonPropertyName("referrer_code")]
        public string ReferrerCode { get; set; } = string.Empty;

        [JsonPropertyName("registered_at")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? RegisteredAt { get; set; }

        [JsonPropertyName("suspended")]
        public bool Suspended { get; set; }

        [JsonPropertyName("updated_at")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("phone_numbers")]
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();

        [JsonPropertyName("user_profile")]
        public UserProfileDto UserProfile { get; set; } = new();
    }
}
