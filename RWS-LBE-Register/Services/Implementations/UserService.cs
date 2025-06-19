using RWS_LBE_Register.Common;
using RWS_LBE_Register.DTOs.Rlp.Requests;
using RWS_LBE_Register.DTOs.User.Shared;
using RWS_LBE_Register.Services.Interfaces;

namespace RWS_LBE_Register.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IRlpService _rlpService;

        public UserService(ILogger<UserService> logger, IRlpService rlpService)
        {
            _logger = logger;
            _rlpService = rlpService;
        }

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

        public async Task UnmapCarDetailExternalIdAsync(HttpContext httpContext, HttpClient httpClient, string externalId)
        {
            _logger.LogInformation("Unmapping car details for external ID: {ExternalId}", externalId);

            var (existingProfile, raw, error) = await _rlpService.GetRlpProfileAsync(httpClient, externalId);
            if (error != null || existingProfile == null)
            {
                _logger.LogWarning("Failed to retrieve existing profile for unmapping. Error: {Error}", error?.Message);
                return;
            }

            if (existingProfile.User.UserProfile.CarDetail.Count > 0)
            {
                var filteredIdentifiers = existingProfile.User.Identifiers
                    .Where(id => id.ExternalIDType != "CAR_PLATE" && id.ExternalIDType != "IU_NUMBER")
                    .ToList();

                var rlpUpdateUserReq = new UserProfileRequest
                {
                    User = new RlpUserRequest
                    {
                        Identifiers = filteredIdentifiers,
                        ClearIdentifiers = true,
                        UserProfile = new UserProfileDto
                        {
                            CarDetail = new List<CarDetailDto>()
                        }
                    }
                };

                var (_, updateRaw, updateErr) = await _rlpService.UpdateRlpProfileAsync(httpClient, externalId, rlpUpdateUserReq);
                if (updateErr != null)
                {
                    _logger.LogError("Failed to update user profile to unmap car details: {Error}", updateErr);
                    HandleRlpError(httpContext, updateRaw);
                    return;
                }
            }
        }

        // Placeholder if you need to handle RLP errors consistently
        private void HandleRlpError(HttpContext context, string? raw)
        {
            // Example logging or response flagging logic
            _logger.LogError("HandleRlpError called. Raw response: {Raw}", raw);
            // You might return or throw depending on your app's error strategy
        }
    }
}
