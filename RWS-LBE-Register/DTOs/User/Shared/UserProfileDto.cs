using System.Collections.Generic;
namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class UserProfileDto
    {
        public string? CountryCode { get; set; }

        public string? CountryName { get; set; }

        public string? PreviousEmail { get; set; }

        public string? ActiveStatus { get; set; }

        public string? LanguagePreference { get; set; }

        public MarketingPreferenceDto MarketingPreference { get; set; } = new();

        public List<CarDetailDto> CarDetail { get; set; } = new();
    }
}
