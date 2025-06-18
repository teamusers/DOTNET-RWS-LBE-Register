using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RWS_LBE_Register.Common;
using RWS_LBE_Register.DTOs.Acs.Requests; 
using RWS_LBE_Register.Services.Interfaces;

namespace RWS_LBE_Register.Services
{
    public class AcsSettings
    {
        public string Host { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }

    public class AcsService
    {
        private const string AcsAuthUrl = "/api/v1/auth";
        private const string AcsSendEmailByTemplateUrl = "/api/v1/send/template/{0}";

        private readonly HttpClient _client;
        private readonly AcsSettings _settings;
        private readonly IAuthService _authService;

        public AcsService(HttpClient client, IOptions<AcsSettings> settings, IAuthService authService)
        {
            _client = client;
            _settings = settings.Value;
            _authService = authService;
        }

        public async Task<ApiResponse> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var signature = _authService.GenerateSignature(_settings.AppId, _settings.Secret);

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.Host}{AcsAuthUrl}")
                {
                    Content = new StringContent(JsonSerializer.Serialize(signature), Encoding.UTF8, "application/json")
                };
                request.Headers.Add("AppID", _settings.AppId);

                var response = await _client.SendAsync(request, cancellationToken);
                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return ResponseTemplate.InternalErrorResponse();

                if (ApiException.TryParseJson<ApiResponse>(json, out var apiResp))
                    return apiResp ?? ResponseTemplate.InternalErrorResponse();

                return ResponseTemplate.InternalErrorResponse();
            }
            catch
            {
                return ResponseTemplate.InternalErrorResponse();
            }
        }

        public async Task<ApiResponse> SendEmailByTemplateAsync(string templateName, AcsSendEmailByTemplateRequest payload, CancellationToken cancellationToken = default)
        {
            try
            {
                var tokenResult = await GetAccessTokenAsync(cancellationToken);
                if (tokenResult.Code != Codes.SUCCESSFUL || tokenResult.Data is not JsonElement dataElement || !dataElement.TryGetProperty("access_token", out var tokenProp))
                    return ResponseTemplate.InternalErrorResponse();

                var token = tokenProp.GetString();
                if (string.IsNullOrEmpty(token))
                    return ResponseTemplate.InternalErrorResponse();

                var url = $"{_settings.Host}{string.Format(AcsSendEmailByTemplateUrl, templateName)}"; 

                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("AppID", _settings.AppId);

                var response = await _client.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return ResponseTemplate.InternalErrorResponse();

                return ResponseTemplate.GenericSuccessResponse(null)
                    ?? ResponseTemplate.InternalErrorResponse();

            }
            catch
            {
                return ResponseTemplate.InternalErrorResponse();
            }
        }
    }
}
