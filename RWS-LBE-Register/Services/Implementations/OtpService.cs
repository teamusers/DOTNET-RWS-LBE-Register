using RWS_LBE_Register.Services.Interfaces;
namespace RWS_LBE_Register.Services.Implementations
{
    // Model
    public class Otp
    {
        public string? OtpCode { get; set; }
        public long OtpExpiry { get; set; }
    }
     

    // Simple implementation without Redis or validation
    public class OTPService : IOtpService
    {
        private readonly TimeSpan _otpExpiration = TimeSpan.FromMinutes(30);

        public Task<Otp> GenerateOTPAsync(string identifier, CancellationToken cancellationToken = default)
        {
            var random = new Random();
            var otpValue = random.Next(100000, 999999).ToString();
            var expiryTimestamp = DateTimeOffset.UtcNow.Add(_otpExpiration).ToUnixTimeSeconds();

            return Task.FromResult(new Otp
            {
                OtpCode = otpValue,
                OtpExpiry = expiryTimestamp
            });
        }
    }
}
