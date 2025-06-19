using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class UserProfileDto
    {
        [JsonPropertyName("country_code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CountryCode { get; set; }

        [JsonPropertyName("country_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CountryName { get; set; }

        [JsonPropertyName("previous_email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PreviousEmail { get; set; }

        [JsonPropertyName("active_status")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ActiveStatus { get; set; }

        [JsonPropertyName("language_preference")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LanguagePreference { get; set; }

        [JsonPropertyName("market_pref_push")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? MarketPrefPush { get; set; }

        [JsonPropertyName("market_pref_email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? MarketPrefEmail { get; set; }

        [JsonPropertyName("market_pref_sms")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? MarketPrefMobile { get; set; }

        [JsonPropertyName("car_detail")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CarDetailDto> CarDetail { get; set; } = new();
    }
}
