using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RWS_LBE_Register.Common;
using RWS_LBE_Register.DTOs.User.Shared;

namespace RWS_LBE_Register.DTOs.User.Requests
{
    public class RegisterUserRequest : IValidatableObject
    {
        [Required]
        [JsonPropertyName("user")]
        public UserDto User { get; set; } = new();

        [Required]
        [JsonPropertyName("sign_up_type")]
        public string SignUpType { get; set; } = string.Empty;

        [JsonPropertyName("reg_id")]
        public string? RegId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!Codes.IsValidSignUpType(SignUpType))
            {
                results.Add(new ValidationResult("invalid sign_up_type provided", new[] { nameof(SignUpType) }));
                return results; // Stop early if invalid type
            }

            if (SignUpType == Codes.SignUpTypeTM)
            {
                if (string.IsNullOrEmpty(User?.EmployeeId))
                    results.Add(new ValidationResult("user.employee_id is required", new[] { "user.employee_id" }));
            }
            else if (SignUpType == Codes.SignUpTypeGRCMS)
            {
                if (string.IsNullOrEmpty(RegId))
                    results.Add(new ValidationResult("reg_id is required", new[] { nameof(RegId) }));
            }
            else
            {
                if (string.IsNullOrEmpty(User?.Email))
                    results.Add(new ValidationResult("user.email is required", new[] { "user.email" }));

                if (string.IsNullOrEmpty(User?.FirstName))
                    results.Add(new ValidationResult("user.first_name is required", new[] { "user.first_name" }));

                if (string.IsNullOrEmpty(User?.LastName))
                    results.Add(new ValidationResult("user.last_name is required", new[] { "user.last_name" }));

                if (User?.Dob == null)
                    results.Add(new ValidationResult("user.dob is required", new[] { "user.dob" }));

                if (User?.PhoneNumbers == null || User.PhoneNumbers.Count == 0 || string.IsNullOrEmpty(User.PhoneNumbers[0].PhoneNumber))
                    results.Add(new ValidationResult("user.phone_numbers must be properly populated", new[] { "user.phone_numbers" }));

                if (string.IsNullOrEmpty(User?.UserProfile?.CountryCode))
                    results.Add(new ValidationResult("user.user_profile.country_code is required", new[] { "user.user_profile.country_code" }));

                if (string.IsNullOrEmpty(User?.UserProfile?.CountryName))
                    results.Add(new ValidationResult("user.user_profile.country_name is required", new[] { "user.user_profile.country_name" }));

                if (SignUpType == Codes.SignUpTypeGR)
                {
                    if (User?.GrProfile == null)
                        results.Add(new ValidationResult("gr_profile is required", new[] { "user.gr_profile" }));
                    else if (string.IsNullOrEmpty(User.GrProfile.Class))
                        results.Add(new ValidationResult("gr_profile.class is required", new[] { "user.gr_profile.class" }));
                }
            }

            return results;
        }
    }
}
