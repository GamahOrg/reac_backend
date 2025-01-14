using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Integration
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Integration)]
    [AllowAnonymous]

    public class MOJIntegrationController : ControllerBase
    {
        private readonly IMOJService _mOJService;

        public MOJIntegrationController(IMOJService mOJService)
        {
            _mOJService = mOJService;
        }

        [HttpPost(nameof(ExternalAttorneyInquiry))]
        public async Task<IActionResult> ExternalAttorneyInquiry(MOJRequestDto mOJRequest)
          =>  Ok(await _mOJService.ExternalAttorneyInquiry(mOJRequest));

    }
}
