using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.LitigationRequestContractType)]

public class SysLitigationRequestContractTypeController : BaseController<SysLitigationRequestContractType,
       SysLitigationRequestContractTypeCreateUpdateDto, SysLitigationRequestContractTypeDto
       , LitigationRequestContractType>
{
    private readonly ISysLitigationRequestContractTypeService _typeService;
    public SysLitigationRequestContractTypeController(ISysLitigationRequestContractTypeService sysLitigationRequestContractTypeService) :
        base(sysLitigationRequestContractTypeService)
    {
        _typeService = sysLitigationRequestContractTypeService;
    }
    [HttpGet("GetTypes")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTypes(CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _typeService.GetListAsync(cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet("GetTermsConditions")]
    public async Task<IActionResult> GetTermsConditions(string contractNumber,CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _typeService.GetTermsConditionsAsync(contractNumber,cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
}