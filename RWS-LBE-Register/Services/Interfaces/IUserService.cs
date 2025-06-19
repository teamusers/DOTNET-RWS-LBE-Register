using RWS_LBE_Register.DTOs.User.Shared;

namespace RWS_LBE_Register.Services.Interfaces
{
    public interface IUserService
    {
        string? AssignTier(UserDto user, string signUpType);
        string? GrTierMatching(string grClass);
        Task UnmapCarDetailExternalIdAsync(HttpContext httpContext, HttpClient httpClient, string externalId);
    }
}
