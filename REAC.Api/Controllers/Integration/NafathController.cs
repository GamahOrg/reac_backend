using Core.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using REAC.Application.AppServices.Integration.Hubs;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Integration.Hubs.Contracts;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Integration
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Integration)]
    [AllowAnonymous]

    public class NafathController : ControllerBase
    {
        private readonly INafathService _nafathService;
        private readonly IHubContext<IdentityHub, IIdentityClient> _identityHub;

        public NafathController(INafathService nafathService, IHubContext<IdentityHub, IIdentityClient> identityHub)
        {
            _nafathService = nafathService;
            _identityHub = identityHub;
        }

        //[HttpPost(nameof(Nafath))]
        //public async Task<IActionResult> Nafath(NafathRequestDto nafathRequestDto, CancellationToken cancellationToken)
        //  => Ok(await _nafathService.Nafath(nafathRequestDto, cancellationToken));


        //[HttpPost(nameof(NafathCheckStatus))]
        //public async Task<IActionResult> NafathCheckStatus()
        //{
        //    await _nafathService.CheckNafathStatus();
        //}
    }
}
