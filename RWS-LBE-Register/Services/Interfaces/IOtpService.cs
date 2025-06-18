using RWS_LBE_Register.DTOs.Requests;
using RWS_LBE_Register.Services.Implementations;

namespace RWS_LBE_Register.Services.Interfaces
{
    public interface IOtpService
    {
        Task<Otp> GenerateOTPAsync(string identifier, CancellationToken cancellationToken = default);
    }
}