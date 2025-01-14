using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.LitigationRequestCategory)]

public class SysExemptionReductionCategoryController : BaseController<SysExemptionReductionCategory,
       SysExemptionReductionCategoryCreateUpdateDto, SysExemptionReductionCategoryDto
       , Guid>
{
    private readonly ISysExemptionReductionCategoryService _exemptionService;
    public SysExemptionReductionCategoryController(ISysExemptionReductionCategoryService sysExemptionReductionCategoryService) :
        base(sysExemptionReductionCategoryService)
    {
        _exemptionService = sysExemptionReductionCategoryService;
    }

    [HttpGet("Get")]
    [AllowAnonymous]
    public override Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return base.Get(cancellationToken);
    }
}