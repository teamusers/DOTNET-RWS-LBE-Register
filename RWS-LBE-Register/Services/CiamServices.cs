using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Options;
using RWS_LBE_Register.DTOs.Ciam.CiamResponses;
using RWS_LBE_Register.DTOs.Ciam.Requests;
using RWS_LBE_Register.DTOs.Shared;
using RWS_LBE_Register.Helpers;

namespace RWS_LBE_Register.Services
{
    public class CiamSettings
    {
        public string AuthHost { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string TenantID { get; set; } = string.Empty;
        public string ClientID { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string UserIdLinkExtensionKey { get; set; } = string.Empty;
        public string DefaultIssuer { get; set; } = string.Empty;
    }

    public class CiamService
    {
        private readonly HttpClient _client;
        private readonly CiamSettings _settings;
        private readonly IApiHttpClient _apiHttpClient;

        public string DefaultIssuer => _settings.DefaultIssuer;
        public string UserIdLinkExtensionKey => _settings.UserIdLinkExtensionKey;

        public CiamService(HttpClient client, IOptions<CiamSettings> settings, IApiHttpClient apiHttpClient)
        {
            _client = client;
            _settings = settings.Value;
            _apiHttpClient = apiHttpClient;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var tokenUrl = $"{_settings.AuthHost.TrimEnd('/')}/{_settings.TenantID}/oauth2/v2.0/token";

            var content = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _settings.ClientID },
                { "client_secret", _settings.ClientSecret },
                { "scope", "https://graph.microsoft.com/.default" }
            };

            var tokenResp = await _apiHttpClient.DoApiRequestAsync<TokenResponse>(new ApiRequestOptions
            {
                Url = tokenUrl,
                Method = HttpMethod.Post,
                ContentType = "application/x-www-form-urlencoded",
                Body = content,
                ExpectedStatus = HttpStatusCode.OK
            });

            return tokenResp?.AccessToken ?? throw new Exception("Unable to retrieve access token.");
        }

        public async Task<GraphUserCollection?> GetUserByEmailAsync(string email)
        {
            var token = await GetAccessTokenAsync();
            var filter = HttpUtility.UrlEncode($"mail eq '{email}'");
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users?$filter={filter}";

            return await _apiHttpClient.DoApiRequestAsync<GraphUserCollection>(new ApiRequestOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                BearerToken = token,
                ExpectedStatus = HttpStatusCode.OK
            });
        }

        public async Task<GraphUserCollection?> GetUserByGrIdAsync(string grId)
        {
            var token = await GetAccessTokenAsync();
            var filter = HttpUtility.UrlEncode($"{_settings.UserIdLinkExtensionKey}/grid eq '{grId}'");
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users?$filter={filter}";

            return await _apiHttpClient.DoApiRequestAsync<GraphUserCollection>(new ApiRequestOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                BearerToken = token,
                ExpectedStatus = HttpStatusCode.OK
            });
        }

        public async Task<GraphCreateUserResponse?> RegisterUserAsync(GraphCreateUserRequest payload)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users";

            return await _apiHttpClient.DoApiRequestAsync<GraphCreateUserResponse>(new ApiRequestOptions
            {
                Url = url,
                Method = HttpMethod.Post,
                Body = payload,
                BearerToken = token,
                ExpectedStatus = HttpStatusCode.Created
            });
        }

        public async Task<bool> PatchUserAsync(string userId, object payload)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users/{userId}";

            try
            {
                await _apiHttpClient.DoApiRequestAsync<object>(new ApiRequestOptions
                {
                    Url = url,
                    Method = HttpMethod.Patch,
                    Body = payload,
                    BearerToken = token,
                    ExpectedStatus = HttpStatusCode.NoContent
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GraphUserIdExtensionValues?> GetUserSchemaExtensionsAsync(string userId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users/{userId}?$select={_settings.UserIdLinkExtensionKey}";

            var dict = await _apiHttpClient.DoApiRequestAsync<Dictionary<string, JsonElement>>(new ApiRequestOptions
            {
                Url = url,
                Method = HttpMethod.Get,
                BearerToken = token,
                ExpectedStatus = HttpStatusCode.OK
            });

            if (dict != null && dict.TryGetValue(_settings.UserIdLinkExtensionKey, out var extVal))
            {
                return extVal.Deserialize<GraphUserIdExtensionValues>();
            }

            return null;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users/{userId}";

            try
            {
                await _apiHttpClient.DoApiRequestAsync<object>(new ApiRequestOptions
                {
                    Url = url,
                    Method = HttpMethod.Delete,
                    BearerToken = token,
                    ExpectedStatus = HttpStatusCode.NoContent
                });

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
