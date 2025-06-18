using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RWS_LBE_Register.DTOs.User.Shared;

namespace RWS_LBE_Register.DTOs.User.Requests
{
    public class VerifyGrUserRequest : IValidatableObject
    {
        [Required]
        [JsonPropertyName("user")]
        public UserDto User { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (User?.GrProfile == null)
            {
                results.Add(new ValidationResult("gr_profile is required", new[] { "user.gr_profile" }));
            }
            else
            {
                if (string.IsNullOrEmpty(User.GrProfile.Id))
                    results.Add(new ValidationResult("gr_profile.id is required", new[] { "user.gr_profile.id" }));

                if (string.IsNullOrEmpty(User.GrProfile.Pin))
                    results.Add(new ValidationResult("gr_profile.pin is required", new[] { "user.gr_profile.pin" }));
            }

            return results;
        }
    }
}
