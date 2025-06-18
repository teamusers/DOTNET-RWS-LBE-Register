using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class UserProfileDto
    {
        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("country_name")]
        public string? CountryName { get; set; }

        [JsonPropertyName("previous_email")]
        public string? PreviousEmail { get; set; }

        [JsonPropertyName("active_status")]
        public string? ActiveStatus { get; set; }

        [JsonPropertyName("language_preference")]
        public string? LanguagePreference { get; set; }

        [JsonPropertyName("market_pref_push")]
        public bool? MarketPrefPush { get; set; }

        [JsonPropertyName("market_pref_email")]
        public bool? MarketPrefEmail { get; set; }

        [JsonPropertyName("market_pref_sms")]
        public bool? MarketPrefMobile { get; set; }

        [JsonPropertyName("car_detail")]
        public List<CarDetailDto> CarDetail { get; set; } = new();
    }
}
