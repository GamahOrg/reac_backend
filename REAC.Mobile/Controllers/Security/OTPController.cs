using Core.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Domain.Shared.Constants;

namespace REAC.Mobile.Controllers.Security
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Security)]
    [AllowAnonymous]

    public class OTPController : ControllerBase
    {
        private readonly IOTPService _oPTService;

        public OTPController(IOTPService oPTService)
        {
            _oPTService = oPTService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(GenerateOTP) + "/{mobileNumber}")]
        public async Task<BaseResponse> GenerateOTP(string mobileNumber)
            => await _oPTService.SaveOTP(mobileNumber);

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(ValidateOTP) + "/{mobileNumber}" + "/{OTP}")]
        public async Task<BaseResponse> ValidateOTP(string mobileNumber, string OTP)
            => await _oPTService.ValidateOTP(mobileNumber, OTP);

        [HttpGet]
        [Route(nameof(GetOTPTimeoutValue))]
        public async Task<BaseResponse> GetOTPTimeoutValue()
        => await _oPTService.GetOTPTimeoutSettingValue();


        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(GetOTPByMobile) + "/{mobileNumber}")]
        public async Task<string> GetOTPByMobile(string mobileNumber)
            => await _oPTService.GetOTPByMobile(mobileNumber);
    }
}
