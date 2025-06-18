namespace RWS_LBE_Register.DTOs.User.Shared
{
    public class RlpUserRequest
    {
        public List<IdentifierDto> Identifier { get; set; } = new();
        public string? Email { get; set; }
        public DateTime? Dob { get; set; }
        public string? Country { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();
        public UserProfileDto UserProfile { get; set; } = new();
    }

    public static class UserMapper
    {
        public static RlpUserRequest MapLbeToRlpUser(UserDto user)
        {
            return new RlpUserRequest
            {
                Identifier = user.Identifier,
                Email = user.Email,
                Dob = user.DateOfBirth,
                Country = user.Country,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumbers = user.PhoneNumbers,
                UserProfile = user.UserProfile
            };
        }
    }

}
