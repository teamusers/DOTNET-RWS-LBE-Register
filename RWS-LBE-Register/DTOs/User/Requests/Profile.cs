using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RWS_LBE_Register.DTOs.User.Shared; 

namespace RWS_LBE_Register.DTOs.Requests
{
    public class UpdateUserProfile
    {
        [JsonPropertyName("user")]
        public UserDto User { get; set; } = new();
    }

    public class GetUserIdExtensions
    { 
        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
    }
}
