using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.AreaDto;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.ArbitratorClass)]
[AllowAnonymous]

public class SysArbitratorClassController : BaseController<SysArbitratorClass,
       SysArbitratorClassDto, SysArbitratorClassDto
       , Guid>
{
    private readonly ISysArbitratorClassService _sysArbitratorClassService;

    public SysArbitratorClassController(ISysArbitratorClassService sysArbitratorClassService) :
         base(sysArbitratorClassService)
    {
        _sysArbitratorClassService = sysArbitratorClassService;
    }


    [HttpGet(nameof(GetArbitratorClasses))]
    public async Task<ActionResult> GetArbitratorClasses(int experienceYears)
    {
        return Ok(await _sysArbitratorClassService.GetArbitratorClasses(experienceYears));
    }

    [HttpGet(nameof(GetArbitratorClass))]
    public async Task<ActionResult> GetArbitratorClass(int experienceYears)
    {
        return Ok(await _sysArbitratorClassService.GetArbitratorClass(experienceYears));
    }

}