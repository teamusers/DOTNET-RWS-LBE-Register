using System.Net; 
using Microsoft.Extensions.Options;
using RWS_LBE_Register.Common;
using RWS_LBE_Register.DTOs.Configurations;
using RWS_LBE_Register.DTOs.Rlp.Requests;
using RWS_LBE_Register.DTOs.Rlp.Responses;
using RWS_LBE_Register.DTOs.Shared;
using RWS_LBE_Register.Exceptions;
using RWS_LBE_Register.Helpers;
using RWS_LBE_Register.Services.Interfaces;

namespace RWS_LBE_Register.Services.Implementations
{
    public class RlpService : IRlpService
    {
        private readonly RlpApiConfig _settings;
        private readonly IApiHttpClient _apiHttpClient;

        public RlpService(IOptions<RlpApiConfig> settings, IApiHttpClient apiHttpClient)
        {
            _settings = settings.Value;
            _apiHttpClient = apiHttpClient;
        }

        public string GetRetailerId() => _settings.RetailerId;

        public async Task<(GetUserResponse?, string?, Exception?)> CreateRlpProfileAsync(HttpClient client, UserProfileRequest payload)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(_settings, _settings.Core.CreateProfileUrl, null, null);

            try
            {
                var result = await _apiHttpClient.DoApiRequestAsync<GetUserResponse>(new ApiRequestOptions
                {
                    Url = url,
                    Method = HttpMethod.Post,
                    Body = payload,
                    BasicAuth = basicAuth,
                    ExpectedStatus = HttpStatusCode.OK
                });

                return (result, null, null);
            }
            catch (ExternalApiException ex)
            {
                return (default, ex.RawResponse, ex);
            }
            catch (Exception ex)
            {
                return (default, null, ex);
            }
        }

        public async Task<(GetUserResponse?, string?, Exception?)> UpdateRlpProfileAsync(HttpClient client, string externalId, UserProfileRequest payload)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(_settings, _settings.Core.ProfileUrl, externalId, null);

            try
            {
                var result = await _apiHttpClient.DoApiRequestAsync<GetUserResponse>(new ApiRequestOptions
                {
                    Url = url,
                    Method = HttpMethod.Put,
                    Body = payload,
                    BasicAuth = basicAuth,
                    ExpectedStatus = HttpStatusCode.OK
                });

                return (result, null, null);
            }
            catch (ExternalApiException ex)
            {
                return (default, ex.RawResponse, ex);
            }
            catch (Exception ex)
            {
                return (default, null, ex);
            }
        }

        public async Task<(GetUserResponse?, string?, Exception?)> GetRlpProfileAsync(HttpClient client, string externalId)
        {
            var query = "user[user_profile]=true&expand_incentives=true&show_identifiers=true";
            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(_settings, _settings.Core.ProfileUrl, externalId, query);

            try
            {
                var result = await _apiHttpClient.DoApiRequestAsync<GetUserResponse>(new ApiRequestOptions
                {
                    Url = url,
                    Method = HttpMethod.Get,
                    BasicAuth = basicAuth,
                    ExpectedStatus = HttpStatusCode.OK
                });

                return (result, null, null);
            }
            catch (ExternalApiException ex)
            {
                return (default, ex.RawResponse, ex);
            }
            catch (Exception ex)
            {
                return (default, null, ex);
            }
        }

        public async Task<(object?, string?, Exception?)> UpdateUserTierAsync(HttpClient client, UserTierUpdateEventRequest payload)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(_settings, _settings.Offers.EventUrl, null, null);

            try
            {
                var result = await _apiHttpClient.DoApiRequestAsync<object>(new ApiRequestOptions
                {
                    Url = url,
                    Method = HttpMethod.Post,
                    Body = payload,
                    BasicAuth = basicAuth,
                    ExpectedStatus = HttpStatusCode.OK
                });

                return (result, null, null);
            }
            catch (ExternalApiException ex)
            {
                return (default, ex.RawResponse, ex);
            }
            catch (Exception ex)
            {
                return (default, null, ex);
            }
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
    }
}
