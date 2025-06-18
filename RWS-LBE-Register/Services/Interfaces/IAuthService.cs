using RWS_LBE_Register.DTOs.Auth.Requests;

namespace RWS_LBE_Register.Services.Interfaces
{
    public interface IAuthService
    {
        AuthRequest GenerateSignature(string appId, string secretKey);
        AuthRequest GenerateSignatureWithParams(
            string appId,
            string nonce,
            string timestamp,
            string secretKey);
    }
}