using System.Text.Json.Serialization;
using RWS_LBE_Register.DTOs.User.Shared;
namespace RWS_LBE_Register.DTOs.Ciam.Requests
{
    public class Identity
    {
        [JsonPropertyName("signInType")]
        public string SignInType { get; set; } = string.Empty;

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; } = string.Empty;

        [JsonPropertyName("issuerAssignedId")]
        public string IssuerAssignedID { get; set; } = string.Empty;
    }

    public class PasswordProfile
    {
        [JsonPropertyName("forceChangePasswordNextSignIn")]
        public bool ForceChangePasswordNextSignIn { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class GraphCreateUserRequest
    {
        [JsonPropertyName("accountEnabled")]
        public bool AccountEnabled { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("mailNickname")]
        public string MailNickname { get; set; } = string.Empty;

        [JsonPropertyName("identities")]
        public List<Identity> Identities { get; set; } = new();

        [JsonPropertyName("mail")]
        public string Mail { get; set; } = string.Empty;

        [JsonPropertyName("passwordProfile")]
        public PasswordProfile PasswordProfile { get; set; } = new();

        [JsonPropertyName("passwordPolicies")]
        public string PasswordPolicies { get; set; } = string.Empty;

        [JsonPropertyName("userType")]
        public string UserType { get; set; } = string.Empty;
    }

    public class GraphDisableAccountRequest
    {
        [JsonPropertyName("accountEnabled")]
        public bool AccountEnabled { get; set; }
    }

    public class UserIdLinkSchemaExtensionFields
    {
        [JsonPropertyName("rlpid")]
        public string RlpId { get; set; } = string.Empty;

        [JsonPropertyName("rlpno")]
        public string RlpNo { get; set; } = string.Empty;

        [JsonPropertyName("grid")]
        public string GrId { get; set; } = string.Empty;
    }

    public static class GraphUserRequestBuilder
    {
        public static GraphCreateUserRequest GenerateInitialRegistrationRequest(UserDto user, string defaultIssuer)
        {
            return new GraphCreateUserRequest
            {
                AccountEnabled = true,
                DisplayName = $"{user.FirstName} {user.LastName}",
                MailNickname = user.Email?.Split('@')[0] ?? string.Empty,
                Mail = user.Email ?? string.Empty,
                UserType = "Guest",
                PasswordPolicies = "DisablePasswordExpiration",
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = false,
                    Password = user.Password ?? "Password123!"
                },
                Identities = new List<Identity>
            {
                new Identity
                {
                    SignInType = "emailAddress",
                    Issuer = defaultIssuer,
                    IssuerAssignedID = user.Email ?? string.Empty
                }
            }
            };
        }
    }

}
