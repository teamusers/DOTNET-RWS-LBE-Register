namespace RWS_LBE_Register.DTOs.Configurations
{
    public class RlpApiConfig
    {
        public string RetailerId { get; set; } = string.Empty;
        public RlpCoreConfig Core { get; set; } = new();
        public RlpOffersConfig Offers { get; set; } = new();
        public RlpEventConfig Events { get; set; } = new();
    }

    public class RlpEventConfig
    {
        public string PublicTier { get; set; } = "PUBLIC_TIER";
        public string MoveTierB { get; set; } = "MOVE_TIER_B";
        public string MoveTierC { get; set; } = "MOVE_TIER_C";
        public string MoveTierD { get; set; } = "MOVE_TIER_D";
        public string DeactivateTier { get; set; } = "DEACTIVATED_TIER";
        public string DeactivateTmTier { get; set; } = "DEACTIVATED_TM_TIER";
    }

    public class RlpCoreConfig
    {
        public string Host { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string CreateProfileUrl { get; set; } = "/priv/v1/apps/:api_key/users";
        public string ProfileUrl { get; set; } = "/priv/v1/apps/:api_key/external/users/:external_id";
    }

    public class RlpOffersConfig
    {
        public string Host { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string EventUrl { get; set; } = "/incentives/api/1.0/user_events/trigger_user_event";
    }
}
