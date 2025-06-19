using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Register.Common;
using RWS_LBE_Register.DTOs.Acs.Requests;
using RWS_LBE_Register.DTOs.Requests;
using RWS_LBE_Register.DTOs.Responses;
using RWS_LBE_Register.DTOs.Rlp.Requests;
using RWS_LBE_Register.DTOs.Rlp.Responses;
using RWS_LBE_Register.Models;
using RWS_LBE_Register.Services;
using RWS_LBE_Register.Services.Interfaces;
using RWS_LBE_Register.Helpers;
using System.Net;
using System.Text.Json.Serialization;
using RWS_LBE_Register.Services.Implementations;
using Azure.Core;
using RWS_LBE_Register.DTOs.User.Responses;
using RWS_LBE_Register.DTOs.User.Shared;
using static System.Net.WebRequestMethods;

namespace RWS_LBE_Register.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class ProfileController : ControllerBase
    {

        private readonly ILogger<UserService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserService _userService;
        private readonly IRlpService _rlpService;
        private readonly CiamService _ciamService;
        private readonly AcsService _acsService;

        public ProfileController(ILogger<UserService> logger, IHttpClientFactory httpClientFactory, IUserService userService, IRlpService rlpService, CiamService ciamService, AcsService acsService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _userService = userService;
            _rlpService = rlpService;
            _ciamService = ciamService;
            _acsService = acsService;
        }

        [HttpGet("{external_id}")] 
        public async Task<IActionResult> GetUserProfile(string external_id)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var (profileResp, raw, error) = await _rlpService.GetRlpProfileAsync(httpClient, external_id);
            if (error != null)
            {
                var errorResponse = _rlpService.HandleRlpError(raw);
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse); 
            }

            if (profileResp == null)
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseTemplate.DefaultResponse(Codes.INTERNAL_ERROR, "RLP get profile failed"));
             
            var responseData = new GetUserProfileResponse
            {
                User = UserMapper.MapRlpToLbeUser(profileResp.User!)
            };

            var resp = new ApiResponse
            {
                Code = Codes.SUCCESSFUL,
                Message = "user found",
                Data = responseData
            };

            return Ok(resp);
        }

        [HttpPut("update/{external_id}")]
        public async Task<IActionResult> UpdateUserProfile(string external_id, [FromBody] UpdateUserProfile req)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var rlpUpdateUserReq = new UserProfileRequest
            {
                User = UserMapper.MapLbeToRlpUser(req.User!)
            };

            if (req.User?.UserProfile?.CarDetail != null && req.User.UserProfile.CarDetail.Any(cd =>
                !string.IsNullOrWhiteSpace(cd.CarPlate) && !string.IsNullOrWhiteSpace(cd.IuNumber)
                && cd.Issg.HasValue && cd.LastUpdated.HasValue
                ))
            {
                
                var car = req.User.UserProfile.CarDetail[0];
                //UnmapCarDetail for steal car plate number.
                await _userService.UnmapCarDetailExternalIdAsync(HttpContext, httpClient, car.CarPlate!);

                // Corrected: always pass the user external_id
                await _userService.UnmapCarDetailExternalIdAsync(HttpContext, httpClient, external_id);

                var carIdentifiers = new List<IdentifierDto>();

                if (!string.IsNullOrEmpty(car.CarPlate))
                {
                    carIdentifiers.Add(new IdentifierDto
                    {
                        ExternalID = car.CarPlate,
                        ExternalIDType = "CAR_PLATE"
                    });
                }
                if (!string.IsNullOrEmpty(car.IuNumber))
                {
                    carIdentifiers.Add(new IdentifierDto
                    {
                        ExternalID = car.IuNumber,
                        ExternalIDType = "IU_NUMBER"
                    });
                }

                rlpUpdateUserReq.User.Identifiers.AddRange(carIdentifiers);
            }
            else
            {
                await _userService.UnmapCarDetailExternalIdAsync(HttpContext, httpClient, external_id);
            }

            var (profileResp, raw, error) = await _rlpService.UpdateRlpProfileAsync(httpClient, external_id, rlpUpdateUserReq);
            if (error != null)
            {
                var errorResponse = _rlpService.HandleRlpError(raw);
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            if (profileResp == null)
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseTemplate.DefaultResponse(Codes.INTERNAL_ERROR, "RLP profile update failed"));


            var responseData = new GetUserProfileResponse
            {
                User = UserMapper.MapRlpToLbeUser(profileResp.User!)
            };
            var resp = new ApiResponse
            {
                Code = Codes.SUCCESSFUL,
                Message = "update successful",
                Data = responseData
            };
            return Ok(resp); 
        }

        [HttpPut("archive/{external_id}")]
        public async Task<IActionResult> WithdrawUserProfile(string external_id)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // 1. Retrieve RLP profile
            var (rlpResp, raw, err) = await _rlpService.GetRlpProfileAsync(httpClient, external_id);
            if (err != null || rlpResp == null)
            {
                var errorResponse = _rlpService.HandleRlpError(raw);
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            var email = rlpResp.User?.Email;
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(ResponseTemplate.DefaultResponse(Codes.INTERNAL_ERROR, "User does not have a previous email."));
            }

            // 2. Lookup CIAM user
            var ciamResp = await _ciamService.GetUserByEmailAsync(email);
            if (ciamResp == null || ciamResp.Value == null || !ciamResp.Value.Any())
            {
                return Conflict(ResponseTemplate.ExistingUserNotFoundErrorResponse());
            }

            var ciamUserId = ciamResp.Value[0].Id;
            var timestamp = DateTime.Now.ToString("yyMMddHHmmss");

            // 3. Update RLP user to withdrawn state
            var rlpUpdateUserReq = new UserProfileRequest
            {
                User = new RlpUserRequest
                {
                    Email = $"{rlpResp.User!.Email}.delete_{timestamp}",
                    UserProfile = new UserProfileDto
                    {
                        ActiveStatus = "0",
                        MarketPrefPush = false,
                        MarketPrefEmail = false,
                        MarketPrefMobile = false 
                    }
                }
            };

            var (profileResp, updateRaw, updateErr) = await _rlpService.UpdateRlpProfileAsync(httpClient, external_id, rlpUpdateUserReq);
            if (updateErr != null)
                return StatusCode((int)HttpStatusCode.InternalServerError, _rlpService.HandleRlpError(updateRaw));

            // 4. Update RLP tier
            var tierEvent = _rlpService.GetWithdrawUserTierEventName(profileResp!.User.Tier);
            var tierReq = new UserTierUpdateEventRequest
            {
                EventLookup = tierEvent,
                UserId = external_id,
                RetailerID = _rlpService.GetRetailerId()
            };

            var (tierResp, tierRaw, tierErr) = await _rlpService.UpdateUserTierAsync(httpClient, tierReq);
            if (tierErr != null)
            {
                var errorResponse = _rlpService.HandleRlpError(tierRaw);
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            // 5. Delete CIAM user
            await _ciamService.DeleteUserAsync(ciamUserId!);
            // 6. Send withdrawal email
            var acsRequest = new AcsSendEmailByTemplateRequest
            {
                Email = email,
                Subject = "Account Withdrawal Confirmation",
                Data = new RequestWithDrawAccountTemplateData
                {
                    Email = email 
                } 
            };

            var sendResult = await _acsService.SendEmailByTemplateAsync("request_email_otp", acsRequest);
            if (sendResult.Code != Codes.SUCCESSFUL)
            {
                _logger.LogError("Failed to send account withdrawal confirmation email. Code: {Code}, Message: {Message}",
                    sendResult.Code, sendResult.Message);
            }

            var responseData = new GetUserProfileResponse
            {
                User = UserMapper.MapRlpToLbeUser(profileResp.User!)
            };

            var resp = new ApiResponse
            {
                Code = Codes.SUCCESSFUL,
                Message = "withdraw successful",
                Data = responseData
            };
            return Ok(resp); 
        } 

        [HttpPost("rlp-id")]
        public async Task<IActionResult> GetUserIdExtensions([FromBody] GetUserIdExtensions request)
        {
            var httpClient = _httpClientFactory.CreateClient();

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(ResponseTemplate.DefaultResponse(Codes.INVALID_REQUEST_BODY, "Email is required."));

            // 1. Find CIAM user
            var ciamResp = await _ciamService.GetUserByEmailAsync(request.Email);
            if (ciamResp == null || ciamResp.Value == null || !ciamResp.Value.Any())
                return Conflict(ResponseTemplate.ExistingUserNotFoundErrorResponse());

            var ciamUserId = ciamResp.Value[0].Id;

            // 2. Get schema extensions
            var idExtensions = await _ciamService.GetUserSchemaExtensionsAsync(ciamUserId!);
            if (idExtensions == null)
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseTemplate.DefaultResponse(Codes.INTERNAL_ERROR, "Unable to retrieve user extensions"));
            
            var resp = new ApiResponse
            {
                Code = Codes.SUCCESSFUL,
                Message = "user found",
                Data = new GetUserIdExtensionsResponseData
                {
                    RlpId = idExtensions.RlpId
                }
            };
            return Ok(resp); 
        } 


    }
}
