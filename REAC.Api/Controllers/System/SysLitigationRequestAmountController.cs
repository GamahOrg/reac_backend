using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.LitigationRequestAmount)]

public class SysLitigationRequestAmountController : BaseController<SysLitigationRequestAmount,
       SysLitigationRequestAmountCreateUpdateDto, SysLitigationRequestAmountCreateUpdateDto
       , Guid>
{
    private readonly ISysLitigationRequestAmountService _amountService;
    public SysLitigationRequestAmountController(ISysLitigationRequestAmountService sysLitigationRequestAmountService) :
        base(sysLitigationRequestAmountService)
    {
        _amountService = sysLitigationRequestAmountService;
    }
    [AllowAnonymous]
    [HttpGet("Get")]
    public override async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var returnValue = await _amountService.GetOrderedListAsync();
        return  Ok(returnValue);
    }
}