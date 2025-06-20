using System.Text.Json;
using RWS_LBE_Register.DTOs.Configurations; 

namespace RWS_LBE_Register.Helpers
{
    public class RlpHelper
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static ((string Username, string Password) BasicAuth, string Url) BuildRlpCoreRequestInfo(RlpApiConfig config, string basePath, string? externalId, string? queryParams)
        {
            string username = config.Core!.ApiKey;
            string password = config.Core!.ApiSecret;

            string url = BuildRlpUrl(config.Core!.Host, config.Core!.ApiKey, basePath, externalId, queryParams);

            return ((username, password), url);
        }

        public static ((string Username, string Password) BasicAuth, string Url) BuildRlpOffersRequestInfo(RlpApiConfig config, string basePath, string? externalId, string? queryParams)
        {
            string username = config.Offers!.ApiKey;
            string password = config.Offers!.ApiSecret;

            string url = BuildRlpUrl(config.Offers!.Host, config.Offers!.ApiKey, basePath, externalId, queryParams);

            return ((username, password), url);
        }

        public static string BuildRlpUrl(string host, string apiKey, string basePath, string? externalId, string? queryParams)
        {
            var endpoint = basePath
                .Replace(":api_key", apiKey)
                .Replace(":external_id", externalId);

            if (!string.IsNullOrWhiteSpace(queryParams))
            {
                endpoint = $"{endpoint}?{queryParams}";
            }

            return $"{host}{endpoint}";
        } 
    }
}