using System.Text.Json.Serialization;

namespace RWS_LBE_Register.DTOs.Ciam.CiamResponses
{
    public static class CiamErrorMessages
    {
        public const string CiamUserPrincipalNameAlreadyExists = "Another object with the same value for property userPrincipalName already exists.";
        public const string CiamUserProxyAddressesAlreadyExists = "Another object with the same value for property proxyAddresses already exists.";
        public const string CiamUserPasswordComplexityInvalid = "The specified password does not comply with password complexity requirements. Please provide a different password.";
    }

    public class TokenResponse
    {
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("ext_expires_in")]
        public int ExtExpiresIn { get; set; }

        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }

    public class GraphUserCollection
    {
        [JsonPropertyName("value")]
        public List<GraphUser>? Value { get; set; }
    }

    public class GraphUser
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("mail")]
        public string? Mail { get; set; }

        [JsonPropertyName("userPrincipalName")]
        public string? UserPrincipalName { get; set; }
    }

    public class GraphCreateUserResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        // other fields are intentionally ignored
    }

    public class GraphUserExtensionCollection
    {
        [JsonPropertyName("value")]
        public List<GraphUserExtension>? Value { get; set; }
    }

    public class GraphUserExtension
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("mail")]
        public string? Mail { get; set; }

        [JsonPropertyName("userPrincipalName")]
        public string? UserPrincipalName { get; set; }
    }

    public class GraphApiErrorResponse
    {
        [JsonPropertyName("error")]
        public GraphApiError? Error { get; set; }
    }

    public class GraphApiError
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class GraphUserIdExtensionValues
    {
        [JsonPropertyName("grid")]
        public string? GrId { get; set; }

        [JsonPropertyName("rlpno")]
        public string? RlpNo { get; set; }

        [JsonPropertyName("rlpid")]
        public string? RlpId { get; set; }
    }
}
