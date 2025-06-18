using System.Text.Json.Serialization;
using RWS_LBE_Register.Helpers;

namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class UserDto
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("password")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Password { get; set; }

        [JsonPropertyName("identifiers")]
        public List<IdentifierDto> Identifiers { get; set; } = new();

        [JsonPropertyName("phone_numbers")]
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("dob")]
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateTime? Dob { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("available_points")]
        public double? AvailablePoints { get; set; }

        [JsonPropertyName("tier")]
        public string? Tier { get; set; }

        [JsonPropertyName("registered_at")]
        public DateTime? RegisteredAt { get; set; }

        [JsonPropertyName("suspended")]
        public bool Suspended { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("user_profile")]
        public UserProfileDto UserProfile { get; set; } = new();

        [JsonPropertyName("gr_profile")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GrProfileDto GrProfile { get; set; } = new();

        [JsonPropertyName("employee_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? EmployeeId { get; set; }
    }
}
