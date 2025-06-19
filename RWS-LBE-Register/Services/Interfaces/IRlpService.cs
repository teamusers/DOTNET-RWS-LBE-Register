using RWS_LBE_Register.Common;
using RWS_LBE_Register.DTOs.Rlp.Requests;
using RWS_LBE_Register.DTOs.Rlp.Responses;

namespace RWS_LBE_Register.Services.Interfaces
{
    public interface IRlpService
    {
        Task<(GetUserResponse? Response, string? RawBody, Exception? Error)> CreateRlpProfileAsync(HttpClient client, UserProfileRequest payload);
        Task<(GetUserResponse? Response, string? RawBody, Exception? Error)> UpdateRlpProfileAsync(HttpClient client, string externalId, UserProfileRequest payload);
        Task<(GetUserResponse? Response, string? RawBody, Exception? Error)> GetRlpProfileAsync(HttpClient client, string externalId);
        Task<(object? Response, string? RawBody, Exception? Error)> UpdateUserTierAsync(HttpClient client, UserTierUpdateEventRequest payload);
        string GetUserTierEventName(string tier);
        string GetWithdrawUserTierEventName(string tier);
        ApiResponse HandleRlpError(string? rawJson);
        string GetRetailerId();
    }
}
