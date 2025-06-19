using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Register.Common;
using RWS_LBE_Register.DTOs.Acs.Requests;
using RWS_LBE_Register.DTOs.Ciam.Requests;
using RWS_LBE_Register.DTOs.Extensions;
using RWS_LBE_Register.DTOs.Rlp.Requests;
using RWS_LBE_Register.DTOs.User.Requests;
using RWS_LBE_Register.DTOs.User.Responses;
using RWS_LBE_Register.Helpers;
using RWS_LBE_Register.Services;
using RWS_LBE_Register.Services.Implementations;
using RWS_LBE_Register.Services.Interfaces;

namespace RWS_LBE_Register.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserService> _logger;
        private readonly IOtpService _otpService;
        private readonly CiamService _ciamService;
        private readonly AcsService _acsService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserService _userService;
        private readonly RlpNumberingHelper _rlpHelper;
        private readonly IRlpService _rlpService;

        public UserController(
            ILogger<UserService> logger,
            IHttpClientFactory httpClientFactory,
            IOtpService otpService,
            CiamService ciamService,
            AcsService acsService,
            IUserService userService,
            RlpNumberingHelper rlpHelper,
            IRlpService rlpService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _otpService = otpService;
            _ciamService = ciamService;
            _acsService = acsService;
            _userService = userService;
            _rlpHelper = rlpHelper;
            _rlpService = rlpService;
        }

        [HttpPost("register/verify")]
        public async Task<IActionResult> VerifyUserExistence([FromBody] VerifyUserExistenceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseTemplate.InvalidRequestBodyErrorResponse());

            bool userExists;
            try
            {
                var userCollection = await _ciamService.GetUserByEmailAsync(request.Email!);
                userExists = userCollection != null && userCollection.Value?.Count > 0;
            }
            catch
            {
                return BadRequest(ResponseTemplate.InternalErrorResponse());
            }

            if (userExists)
                return Conflict(ResponseTemplate.ExistingUserFoundErrorResponse());

            var otp = await _otpService.GenerateOTPAsync(request.Email!);

            var payload = new AcsSendEmailByTemplateRequest
            {
                Email = request.Email,
                Subject = "RWS Loyalty Program - Verify OTP",
                Data = new RequestEmailOtpTemplateData
                {
                    Email = request.Email,
                    Otp = otp.OtpCode
                }
            };

            var sendResult = await _acsService.SendEmailByTemplateAsync("request_email_otp", payload);
            if (sendResult.Code != Codes.SUCCESSFUL)
            {
                _logger.LogError("Failed to send OTP email. Code: {Code}, Message: {Message}",
                    sendResult.Code, sendResult.Message);
            }
            return Ok(ResponseTemplate.GenericSuccessResponse(otp));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseTemplate.InvalidRequestBodyErrorResponse());

            var context = new ValidationContext(request);
            var validationResults = request.Validate(context).ToList();
            if (validationResults.Any())
                return BadRequest(ResponseTemplate.InvalidRequestBodySpecificErrorResponse(string.Join(", ", validationResults.Select(v => v.ErrorMessage))));

            var httpClient = _httpClientFactory.CreateClient();

            switch (request.SignUpType)
            {
                case Codes.SignUpTypeGRCMS:
                    /* To be fix for GRCMS
                    var cachedProfile = await _userService.GetCachedProfileAsync(request.RegId!);
                    if (cachedProfile == null)
                        return Conflict(ResponseTemplate.CachedProfileNotFoundErrorResponse());
                    request.User = cachedProfile;
                    */
                    break;

                case Codes.SignUpTypeTM:
                    request.User.EmployeeId = "TBC";
                    break;
            }

            var assignError = _userService.AssignTier(request.User, request.SignUpType);
            if (!string.IsNullOrWhiteSpace(assignError))
                return Conflict(ResponseTemplate.InvalidGrMemberClassErrorResponse());

            var newRlp = await _rlpHelper.GenerateNextRLPUserNumberingWithRetryAsync();
            if (newRlp == null)
                return StatusCode((int)HttpStatusCode.InternalServerError, ResponseTemplate.InternalErrorResponse());

            request.User.PopulateIdentifiers(newRlp.RLP_ID, newRlp.RLP_NO);
            var rlpUser = UserMapper.MapLbeToRlpUser(request.User);
            rlpUser.PopulateRegistrationDefaults(newRlp.RLP_ID);

            var defaultIssuer = _ciamService.DefaultIssuer;
            var graphCreateUserRequest = GraphUserRequestBuilder.GenerateInitialRegistrationRequest(request.User, defaultIssuer);

            var graphUser = await _ciamService.RegisterUserAsync(graphCreateUserRequest);
            if (graphUser == null)
                return StatusCode((int)HttpStatusCode.InternalServerError, ResponseTemplate.DefaultResponse(Codes.INTERNAL_ERROR, "CIAM user creation failed"));

            var extensionPayload = new Dictionary<string, object>
            {
                [_ciamService.UserIdLinkExtensionKey] = new UserIdLinkSchemaExtensionFields
                {
                    RlpId = newRlp.RLP_ID,
                    RlpNo = newRlp.RLP_NO,
                    GrId = request.User.GrProfile?.Id ?? string.Empty
                }
            };

            var patchSuccess = await _ciamService.PatchUserAsync(graphUser.Id!, extensionPayload);
            if (!patchSuccess)
                return StatusCode((int)HttpStatusCode.InternalServerError, ResponseTemplate.DefaultResponse(Codes.INTERNAL_ERROR, "Failed to patch CIAM schema extensions"));

            rlpUser.ExternalID = newRlp.RLP_ID;
            rlpUser.ExternalIDType = "RLP_ID";

            var initialProfileReq = new UserProfileRequest
            {
                User = rlpUser
            };

            var (initResp, initRaw, initErr) = await _rlpService.CreateRlpProfileAsync(httpClient, initialProfileReq);
            if (initErr != null)
            {
                var errorResponse = _rlpService.HandleRlpError(initRaw);
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            var updateReq = new UserProfileRequest { User = rlpUser };
            var (profileResp, updateRaw, updateErr) = await _rlpService.UpdateRlpProfileAsync(httpClient, newRlp.RLP_ID, updateReq);
            if (updateErr != null)
            {
                var errorResponse = _rlpService.HandleRlpError(updateRaw);
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            var tierReq = new UserTierUpdateEventRequest
            {
                EventLookup = _rlpService.GetUserTierEventName(request.User.Tier!),
                UserId = newRlp.RLP_ID,
                RetailerID = _rlpService.GetRetailerId()
            };

            var (tierResp, tierRaw, tierErr) = await _rlpService.UpdateUserTierAsync(httpClient, tierReq);
            if (tierErr != null)
            {
                var errorResponse = _rlpService.HandleRlpError(tierRaw);
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            if (profileResp == null)
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseTemplate.DefaultResponse(Codes.INTERNAL_ERROR, "RLP profile update failed"));

            if (!string.IsNullOrEmpty(request.User.Tier) && profileResp.User != null)
            {
                profileResp.User.Tier = request.User.Tier;
            }

            var responseData = new CreateUserResponseData
            {
                User = UserMapper.MapRlpToLbeUser(profileResp.User!)
            };
            var resp = new ApiResponse
            {
                Code = Codes.SUCCESSFUL,
                Message = "user created",
                Data = responseData
            };

            /* To be fix for GRCMS
            // purge regId cache if used
            if (request.SignUpType == Codes.SignUpTypeGRCMS)
                await _userService.DeleteCachedProfileAsync(request.RegId!);
            */
            return Created(string.Empty, resp);
        }
    }
}
