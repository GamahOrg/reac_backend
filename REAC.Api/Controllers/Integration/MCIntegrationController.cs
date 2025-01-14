using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Integration
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Integration)]
    [AllowAnonymous]

    public class MCIntegrationController : ControllerBase
    {
        private readonly IMCService _mCService;

        public MCIntegrationController(IMCService mCService)
        {
            _mCService = mCService;
        }

        [HttpGet(nameof(GetCRInformationByCRNumber))]
        public async Task<IActionResult> GetCRInformationByCRNumber(string cRNumber)
          =>  Ok(await _mCService.GetCRInformationByCRNumber(cRNumber));
        
        [HttpGet(nameof(GetCRInformationByCROwners))]
        public async Task<IActionResult> GetCRInformationByCROwners(string cRNumber)
          =>  Ok(await _mCService.GetCRInformationByCROwners(cRNumber));

    }
}
