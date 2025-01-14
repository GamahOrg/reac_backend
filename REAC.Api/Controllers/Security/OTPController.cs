using Core.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Domain.Shared.Constants;
using REAC.Application.Contracts.Dtos.Zoom;

namespace REAC.Api.Controllers.Security
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Security)]
    [AllowAnonymous]

    public class OTPController : ControllerBase
    {
        private readonly IOTPService _oPTService;
        private readonly IZoomService _zoomService;

        public OTPController(IOTPService oPTService, IZoomService zoomService)
        {
            _oPTService = oPTService;
            _zoomService = zoomService;
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


        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(GenerateZoomMeeting))]
        public async Task<ZoomMeting?> GenerateZoomMeeting()
            => await _zoomService.GenerateMeeting("test meeting");

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(GetZoomMeeting))]
        public async Task<BaseResponse> GetZoomMeeting(long MeetingId)
            => await _zoomService.GetMeetingById(MeetingId);

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(GetMeetingParticipantsById))]
        public async Task<List<ZoomMeetingParticipantsDetails?>> GetMeetingParticipantsById(long MeetingId)
            => await _zoomService.GetMeetingParticipantsById(MeetingId);
    }
}
