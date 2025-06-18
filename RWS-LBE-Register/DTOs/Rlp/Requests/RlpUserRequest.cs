using RWS_LBE_Register.Helpers;
using System.Text.Json.Serialization;
using RWS_LBE_Register.DTOs.User.Shared;

namespace RWS_LBE_Register.DTOs.Rlp.Requests
{
    public class RlpUserRequest
    {
        [JsonPropertyName("external_id")]
        public string? ExternalID { get; set; }

        [JsonPropertyName("opted_in")]
        public bool OptedIn { get; set; }

        [JsonPropertyName("external_id_type")]
        public string? ExternalIDType { get; set; }

        [JsonPropertyName("identifiers")]
        public List<IdentifierDto> Identifiers { get; set; } = new();

        [JsonPropertyName("clear_identifiers")]
        public bool ClearIdentifiers { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        [JsonPropertyName("ip")]
        public string? IP { get; set; }

        [JsonPropertyName("dob")]
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateTime? Dob { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("address2")]
        public string? Address2 { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("zip")]
        public string? Zip { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("phone_numbers")]
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();

        [JsonPropertyName("referral")]
        public ReferralDto? Referral { get; set; }

        [JsonPropertyName("user_profile")]
        public UserProfileDto UserProfile { get; set; } = new();

        public void PopulateRegistrationDefaults(string rlpId)
        {
            OptedIn = true;
            ExternalID = rlpId;
            ExternalIDType = "RLP_ID";

            if (UserProfile == null)
                UserProfile = new UserProfileDto();

            UserProfile.LanguagePreference = "EN";
            UserProfile.PreviousEmail = Email;
            UserProfile.ActiveStatus = "1";
        }
    }

    public class ReferralDto
    {
        [JsonPropertyName("referral_code")]
        public string? ReferralCode { get; set; }
    }
}
