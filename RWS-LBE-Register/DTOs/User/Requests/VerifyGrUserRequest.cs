using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RWS_LBE_Register.DTOs.User.Shared;

namespace RWS_LBE_Register.DTOs.User.Requests
{
    public class VerifyGrUserRequest
    {
        [Required]
        public UserDto User { get; set; } = new();

        public IEnumerable<ValidationResult> Validate()
        {
            var results = new List<ValidationResult>();

            if (User?.GrProfile == null)
                results.Add(new ValidationResult("gr_profile is required"));
            else
            {
                if (string.IsNullOrEmpty(User.GrProfile.Id))
                    results.Add(new ValidationResult("gr_profile.id is required"));

                if (string.IsNullOrEmpty(User.GrProfile.Pin))
                    results.Add(new ValidationResult("gr_profile.pin is required"));
            }

            return results;
        }
    }
}
