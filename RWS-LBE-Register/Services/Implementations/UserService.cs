using RWS_LBE_Register.Common; 
using RWS_LBE_Register.Services.Interfaces;
using RWS_LBE_Register.DTOs.User.Shared;

namespace RWS_LBE_Register.Services.Implementations
{
    public class UserService : IUserService
    {
        public string? AssignTier(UserDto user, string signUpType)
        {
            user.Tier = "Tier A";

            if (signUpType == Codes.SignUpTypeGRCMS || signUpType == Codes.SignUpTypeGR)
            {
                var tier = GrTierMatching(user.GrProfile?.Class ?? string.Empty);
                if (tier == null)
                {
                    return "error matching gr class to member tier";
                }

                user.Tier = tier;
            }
            else if (signUpType == Codes.SignUpTypeTM)
            {
                user.Tier = "Tier M";
            }

            return null;
        }

        public string? GrTierMatching(string grClass)
        {
            if (!int.TryParse(grClass.Trim(), out var classLevel) || classLevel < 1)
            {
                return null;
            }

            var tierA = new HashSet<int> { 1 };
            var tierB = new HashSet<int> { 12, 18 };
            var tierC = new HashSet<int> { 13, 14, 19, 20, 25, 26 };
            var tierD = new HashSet<int> { 15, 16, 21, 27 };

            if (tierA.Contains(classLevel)) return "Tier A";
            if (tierB.Contains(classLevel)) return "Tier B";
            if (tierC.Contains(classLevel)) return "Tier C";
            if (tierD.Contains(classLevel)) return "Tier D";

            return null;
        }
    }
}
