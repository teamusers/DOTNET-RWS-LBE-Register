using System.ComponentModel.DataAnnotations;

namespace RWS_LBE_Register.DTOs.User.Requests
{
    public class VerifyCiamUserRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
