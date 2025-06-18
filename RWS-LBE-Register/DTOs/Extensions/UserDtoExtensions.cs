using RWS_LBE_Register.DTOs.User.Shared;
namespace RWS_LBE_Register.DTOs.Extensions
{
    public static class UserDtoExtensions
    {
        public static void PopulateIdentifiers(this UserDto user, string rlpId, string rlpNo)
        {
            if (user.Identifiers == null)
                user.Identifiers = new List<IdentifierDto>();

            // Always add RLP_ID and RLP_NO
            user.Identifiers.Add(new IdentifierDto
            {
                ExternalID = rlpId,
                ExternalIDType = "RLP_ID"
            });

            user.Identifiers.Add(new IdentifierDto
            {
                ExternalID = rlpNo,
                ExternalIDType = "RLP_NO"
            });

            // Add EMPLOYEE_ID if present
            if (!string.IsNullOrWhiteSpace(user.EmployeeId))
            {
                user.Identifiers.Add(new IdentifierDto
                {
                    ExternalID = user.EmployeeId,
                    ExternalIDType = "EMPLOYEE_ID"
                });
            }

            // Add GR_ID if GrProfile and Id exist
            if (user.GrProfile != null && !string.IsNullOrWhiteSpace(user.GrProfile.Id))
            {
                user.Identifiers.Add(new IdentifierDto
                {
                    ExternalID = user.GrProfile.Id,
                    ExternalIDType = "GR_ID"
                });
            }
        }
    }
}
