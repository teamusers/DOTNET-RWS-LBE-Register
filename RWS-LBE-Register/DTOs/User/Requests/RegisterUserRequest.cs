using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RWS_LBE_Register.Common;
using RWS_LBE_Register.DTOs.User.Shared;

namespace RWS_LBE_Register.DTOs.User.Requests
{
    public class RegisterUserRequest
    {
        [Required]
        public UserDto User { get; set; } = new();

        [Required]
        public string SignUpType { get; set; } = string.Empty;

        public string? RegId { get; set; }

        public IEnumerable<ValidationResult> Validate()
        {
            var results = new List<ValidationResult>();

            if (!Codes.IsValidSignUpType(SignUpType))
            {
                results.Add(new ValidationResult("invalid sign_up_type provided"));
            }

            if (SignUpType == Codes.SignUpTypeTM)
            {
                if (string.IsNullOrEmpty(User?.EmployeeId))
                    results.Add(new ValidationResult("user.employee_id is required"));
            }
            else if (SignUpType == Codes.SignUpTypeGRCMS)
            {
                if (string.IsNullOrEmpty(RegId))
                    results.Add(new ValidationResult("reg_id is required"));
            }
            else
            {
                if (string.IsNullOrEmpty(User?.Email))
                    results.Add(new ValidationResult("user.email is required"));

                if (string.IsNullOrEmpty(User?.FirstName))
                    results.Add(new ValidationResult("user.first_name is required"));

                if (string.IsNullOrEmpty(User?.LastName))
                    results.Add(new ValidationResult("user.last_name is required"));

                if (User?.DateOfBirth == null)
                    results.Add(new ValidationResult("user.dob is required"));

                if (User?.PhoneNumbers == null || User.PhoneNumbers.Count == 0 || string.IsNullOrEmpty(User.PhoneNumbers[0].PhoneNumber))
                    results.Add(new ValidationResult("user.phone_numbers must be properly populated"));

                if (string.IsNullOrEmpty(User?.UserProfile?.CountryCode))
                    results.Add(new ValidationResult("user.user_profile.country_code is required"));

                if (string.IsNullOrEmpty(User?.UserProfile?.CountryName))
                    results.Add(new ValidationResult("user.user_profile.country_name is required"));

                if (SignUpType == Codes.SignUpTypeGR)
                {
                    if (User?.GrProfile == null)
                        results.Add(new ValidationResult("gr_profile is required"));
                    else if (string.IsNullOrEmpty(User.GrProfile.Class))
                        results.Add(new ValidationResult("gr_profile.class is required"));
                }
            }

            return results;
        }
    }
}
