using System.Net.Http.Headers;
using System.Text;
using System.Text.Json; 
using System.Web;
using Microsoft.Extensions.Options;
using System.Net;
using RWS_LBE_Register.DTOs.Ciam.Requests; 
using RWS_LBE_Register.DTOs.Ciam.CiamResponses;

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
        public string DefaultIssuer => _settings.DefaultIssuer;
        public string UserIdLinkExtensionKey => _settings.UserIdLinkExtensionKey;

        public CiamService(HttpClient client, IOptions<CiamSettings> settings)
        {
            _client = client;
            _settings = settings.Value;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var tokenUrl = $"{_settings.AuthHost.TrimEnd('/')}/{_settings.TenantID}/oauth2/v2.0/token";

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _settings.ClientID },
                { "client_secret", _settings.ClientSecret },
                { "scope", "https://graph.microsoft.com/.default" }
            });

            var response = await _client.PostAsync(tokenUrl, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);
            return tokenResponse?.AccessToken ?? throw new Exception("Unable to retrieve access token.");
        }

        public async Task<GraphUserCollection?> GetUserByEmailAsync(string email)
        {
            var token = await GetAccessTokenAsync();
            var filter = HttpUtility.UrlEncode($"mail eq '{email}'");
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users?$filter={filter}";
            return await GetAsync<GraphUserCollection>(url, token);
        }

        public async Task<GraphUserCollection?> GetUserByGrIdAsync(string grId)
        {
            var token = await GetAccessTokenAsync();
            var filter = HttpUtility.UrlEncode($"{_settings.UserIdLinkExtensionKey}/grid eq '{grId}'");
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users?$filter={filter}";
            return await GetAsync<GraphUserCollection>(url, token);
        }

        public async Task<GraphCreateUserResponse?> RegisterUserAsync(GraphCreateUserRequest payload)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users";
            return await PostAsync<GraphCreateUserResponse>(url, payload, token);
        }

        public async Task<bool> PatchUserAsync(string userId, object payload)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users/{userId}";

            var req = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.SendAsync(req);
            return res.StatusCode == HttpStatusCode.NoContent;
        }

        public async Task<GraphUserIdExtensionValues?> GetUserSchemaExtensionsAsync(string userId)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_settings.Host.TrimEnd('/')}/v1.0/users/{userId}?$select={_settings.UserIdLinkExtensionKey}";

            var dict = await GetAsync<Dictionary<string, JsonElement>>(url, token);
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

            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.SendAsync(req);
            return res.StatusCode == HttpStatusCode.NoContent;
        }

        private async Task<T?> GetAsync<T>(string url, string token)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return default;

            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json);
        }

        private async Task<T?> PostAsync<T>(string url, object payload, string token)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return default;

            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
