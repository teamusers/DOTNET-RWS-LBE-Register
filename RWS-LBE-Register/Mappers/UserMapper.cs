using RWS_LBE_Register.DTOs.Rlp.Requests;
using RWS_LBE_Register.DTOs.Rlp.Responses;
using RWS_LBE_Register.DTOs.User.Shared;
public static class UserMapper
{
    public static RlpUserRequest MapLbeToRlpUser(UserDto user)
    {
        return new RlpUserRequest
        {
            Identifiers = user.Identifiers,
            Email = user.Email,
            Dob = user.Dob,
            Country = user.Country,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumbers = user.PhoneNumbers,
            UserProfile = user.UserProfile
        };
    }

    public static UserDto MapRlpToLbeUser(RlpUserResp rlpUser)
    {
        return new UserDto
        {
            Email = rlpUser.Email,
            Identifiers = rlpUser.Identifiers,
            PhoneNumbers = rlpUser.PhoneNumbers,
            FirstName = rlpUser.FirstName,
            LastName = rlpUser.LastName,
            Dob = rlpUser.Dob,
            CreatedAt = rlpUser.CreatedAt,
            Country = rlpUser.Country,
            AvailablePoints = rlpUser.AvailablePoints,
            Tier = rlpUser.Tier,
            RegisteredAt = rlpUser.RegisteredAt,
            Suspended = rlpUser.Suspended,
            UpdatedAt = rlpUser.UpdatedAt,
            UserProfile = rlpUser.UserProfile
        };
    }
}
