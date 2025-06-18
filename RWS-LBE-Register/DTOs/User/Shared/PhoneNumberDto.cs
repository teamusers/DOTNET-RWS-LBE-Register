using System.Collections.Generic;
namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class PhoneNumberDto
    {
        public string? PhoneNumber { get; set; }

        public string? PhoneType { get; set; }

        public List<string> PreferenceFlags { get; set; } = new();

        public bool VerifiedOwnership { get; set; }
    }
}
