using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.Acs.Requests
{
    public class AcsSendEmailByTemplateRequest
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("data")]
        public object? Data { get; set; }
    }

    public class RequestEmailOtpTemplateData
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("otp")]
        public string? Otp { get; set; }
    }

    public class RequestEmailOtpWithRegisterLinkTemplateData
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("otp")]
        public string? Otp { get; set; }

        [JsonPropertyName("registerLink")]
        public string? RegisterLink { get; set; }
    }
}
