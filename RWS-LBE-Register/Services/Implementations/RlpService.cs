using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RWS_LBE_Register.DTOs.Rlp.Requests;
using RWS_LBE_Register.DTOs.Rlp.Responses;
using RWS_LBE_Register.Services.Interfaces;
using RWS_LBE_Register.DTOs.Configurations;
using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Register.Common;

namespace RWS_LBE_Register.Services.Implementations
{
    public class RlpService : IRlpService
    {
        private readonly RlpApiConfig _settings;
        public string GetRetailerId() => _settings.RetailerId;

        public RlpService(IOptions<RlpApiConfig> settings)
        {
            _settings = settings.Value;
        }

        public async Task<(GetUserResponse?, string?, Exception?)> CreateRlpProfileAsync(HttpClient client, UserProfileRequest payload)
        {
            var url = BuildProfileUrl(_settings.Core.CreateProfileUrl);
            return await SendRlpApiRequestAsync<GetUserResponse>(client, HttpMethod.Post, url, payload, _settings.Core.ApiKey, _settings.Core.ApiSecret);
        }

        public async Task<(GetUserResponse?, string?, Exception?)> UpdateRlpProfileAsync(HttpClient client, string externalId, UserProfileRequest payload)
        {
            var url = BuildProfileUrl(_settings.Core.ProfileUrl, externalId);
            return await SendRlpApiRequestAsync<GetUserResponse>(client, HttpMethod.Put, url, payload, _settings.Core.ApiKey, _settings.Core.ApiSecret);
        }

        public async Task<(GetUserResponse?, string?, Exception?)> GetRlpProfileAsync(HttpClient client, string externalId)
        {
            var query = "user[user_profile]=true&expand_incentives=true&show_identifiers=true";
            var url = BuildProfileUrl(_settings.Core.ProfileUrl, externalId, query);
            return await SendRlpApiRequestAsync<GetUserResponse>(client, HttpMethod.Get, url, null, _settings.Core.ApiKey, _settings.Core.ApiSecret);
        }

        public async Task<(object?, string?, Exception?)> UpdateUserTierAsync(HttpClient client, UserTierUpdateEventRequest payload)
        {
            var url = $"{_settings.Offers.Host}{_settings.Offers.EventUrl}";
            return await SendRlpApiRequestAsync<object>(client, HttpMethod.Post, url, payload, _settings.Offers.ApiKey, _settings.Offers.ApiSecret);
        }

        public string GetUserTierEventName(string tier)
        {
            return tier switch
            {
                "Tier A" => _settings.Events.PublicTier,
                "Tier B" => _settings.Events.MoveTierB,
                "Tier C" => _settings.Events.MoveTierC,
                "Tier D" => _settings.Events.MoveTierD,
                _ => _settings.Events.PublicTier
            };
        }

        public string GetWithdrawUserTierEventName(string tier)
        {
            return tier switch
            {
                "Tier M" => _settings.Events.DeactivateTmTier,
                _ => _settings.Events.DeactivateTier
            };
        }

        public ApiResponse HandleRlpError(string? rawJson)
        {
            if (string.IsNullOrEmpty(rawJson))
                return ResponseTemplate.InternalErrorResponse();

            if (!ApiException.TryParseJson<UserProfileErrorResponse>(rawJson, out var errResp) || errResp?.Errors == null)
                return ResponseTemplate.InternalErrorResponse();

            return errResp.Errors.Code switch
            {
                RlpErrorCodes.UserNotFound => ResponseTemplate.ExistingUserNotFoundErrorResponse(),
                _ => ResponseTemplate.UnmappedRLPErrorResponse(errResp)
            };
        }

        private string BuildProfileUrl(string basePath, string externalId = "", string queryParams = "")
        {
            var url = basePath.Replace(":api_key", _settings.Core.ApiKey);
            if (!string.IsNullOrEmpty(externalId))
                url = $"{url}/{externalId}";

            if (!string.IsNullOrEmpty(queryParams))
                url = $"{url}?{queryParams}";

            return $"{_settings.Core.Host.TrimEnd('/')}{url}";
        }

        private async Task<(TResponse?, string?, Exception?)> SendRlpApiRequestAsync<TResponse>(HttpClient client, HttpMethod method, string url, object? payload, string username, string password)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);

                if (payload != null)
                {
                    var json = JsonSerializer.Serialize(payload); 
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return (default, responseBody, null);

                var result = JsonSerializer.Deserialize<TResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (result, responseBody, null);
            }
            catch (Exception ex)
            {
                return (default, null, ex);
            }
        }
    }
}
