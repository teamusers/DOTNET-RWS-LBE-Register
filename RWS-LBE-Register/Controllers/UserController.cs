using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Register.Common; 
using RWS_LBE_Register.DTOs.User.Requests;
using RWS_LBE_Register.DTOs.Acs.Requests;
using RWS_LBE_Register.Services;
using RWS_LBE_Register.Services.Interfaces;

namespace RWS_LBE_Register.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    { 
        private readonly IOtpService _otpService;
        private readonly CiamService _ciamService;
        private readonly AcsService _acsService;

        public UserController(
            IHttpClientFactory httpClientFactory,
            IOtpService otpService,
            CiamService ciamService,
            AcsService acsService)
        { 
            _otpService = otpService;
            _ciamService = ciamService;
            _acsService = acsService;
        }
         
        [HttpPost("register/verify")] 
        public async Task<IActionResult> VerifyUserExistence([FromBody] VerifyUserExistenceRequest request)
        {
            // 1) Validate request body
            if (!ModelState.IsValid)
                return BadRequest(ResponseTemplate.InvalidRequestBodyErrorResponse());

            // 2) Check if user exists via injected CIAM service
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

            // 3) Generate OTP
            var otp = await _otpService.GenerateOTPAsync(request.Email!);

            // 4) Prepare and send email using AcsService
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
                return BadRequest(ResponseTemplate.DefaultResponse(Codes.INTERNAL_ERROR, "failed to send OTP email"));

            // 5) Success response
            return Ok(ResponseTemplate.GenericSuccessResponse(otp));
        }
    }
}
