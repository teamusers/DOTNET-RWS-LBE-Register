using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class UserDto
    {
        public string? Email { get; set; }

        public string? Password { get; set; }

        public List<IdentifierDto> Identifier { get; set; } = new();

        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Country { get; set; }

        public double? AvailablePoints { get; set; }

        public string? Tier { get; set; }

        public DateTime? RegisteredAt { get; set; }

        public bool Suspended { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public UserProfileDto UserProfile { get; set; } = new();

        public GrProfileDto GrProfile { get; set; } = new();

        public string? EmployeeId { get; set; }
    }
}
