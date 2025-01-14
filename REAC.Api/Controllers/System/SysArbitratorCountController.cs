using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.ArbitratorCount)]
[AllowAnonymous]

public class SysArbitratorCountController : BaseController<SysArbitratorCount,
       SysArbitratorCountCreateUpdateDto, SysArbitratorCountCreateUpdateDto
       , Guid>
{
    private readonly ISysArbitratorCountService _sysArbitratorCountService;
    public SysArbitratorCountController(ISysArbitratorCountService sysArbitratorCountService) :
         base(sysArbitratorCountService)
    {
        _sysArbitratorCountService = sysArbitratorCountService;
    }
    [AllowAnonymous]
    [HttpGet("Get")]
    public override Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return base.Get(cancellationToken);
    }
}