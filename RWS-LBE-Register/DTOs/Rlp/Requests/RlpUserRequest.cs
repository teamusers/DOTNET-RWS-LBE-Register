using System.Text.Json.Serialization;
using RWS_LBE_Register.DTOs.User.Shared;
using RWS_LBE_Register.Helpers;

namespace RWS_LBE_Register.DTOs.Rlp.Requests
{
    public class RlpUserRequest
    {
        [JsonPropertyName("external_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ExternalID { get; set; }

        [JsonPropertyName("opted_in")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? OptedIn { get; set; }

        [JsonPropertyName("external_id_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ExternalIDType { get; set; }

        [JsonPropertyName("identifiers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<IdentifierDto> Identifiers { get; set; } = new();

        [JsonPropertyName("clear_identifiers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ClearIdentifiers { get; set; }

        [JsonPropertyName("email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Email { get; set; }

        [JsonPropertyName("locale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locale { get; set; }

        [JsonPropertyName("ip")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IP { get; set; }

        [JsonPropertyName("dob")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateTime? Dob { get; set; }

        [JsonPropertyName("address")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Address { get; set; }

        [JsonPropertyName("address2")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Address2 { get; set; }

        [JsonPropertyName("city")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? State { get; set; }

        [JsonPropertyName("zip")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Zip { get; set; }

        [JsonPropertyName("country")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Country { get; set; }

        [JsonPropertyName("gender")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Gender { get; set; }

        [JsonPropertyName("first_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LastName { get; set; }

        [JsonPropertyName("phone_numbers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PhoneNumberDto>? PhoneNumbers { get; set; }

        [JsonPropertyName("referral")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ReferralDto? Referral { get; set; }

        [JsonPropertyName("user_profile")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserProfileDto? UserProfile { get; set; }

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
