using RWS_LBE_Register.DTOs.User.Shared;
namespace RWS_LBE_Register.DTOs.Extensions
{
    public static class UserDtoExtensions
    {
        public static void PopulateIdentifiers(this UserDto user, string rlpId, string rlpNo)
        {
            if (user.Identifier == null)
                user.Identifier = new List<IdentifierDto>();

            // Always add RLP_ID and RLP_NO
            user.Identifier.Add(new IdentifierDto
            {
                ExternalID = rlpId,
                ExternalIDType = "RLP_ID"
            });

            user.Identifier.Add(new IdentifierDto
            {
                ExternalID = rlpNo,
                ExternalIDType = "RLP_NO"
            });

            // Add EMPLOYEE_ID if present
            if (!string.IsNullOrWhiteSpace(user.EmployeeId))
            {
                user.Identifier.Add(new IdentifierDto
                {
                    ExternalID = user.EmployeeId,
                    ExternalIDType = "EMPLOYEE_ID"
                });
            }

            // Add GR_ID if GrProfile and Id exist
            if (user.GrProfile != null && !string.IsNullOrWhiteSpace(user.GrProfile.Id))
            {
                user.Identifier.Add(new IdentifierDto
                {
                    ExternalID = user.GrProfile.Id,
                    ExternalIDType = "GR_ID"
                });
            }
        }
    }
}
