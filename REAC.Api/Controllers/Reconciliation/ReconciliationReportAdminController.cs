using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Reconciliation.Process;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Reconciliation)]
[PermissionRequired(Modules.Reconciliation)]
[AllowAnonymous]

public class ReconciliationReportAdminController : ControllerBase
{
    private readonly IReconciliationService _reconciliationService;

    public ReconciliationReportAdminController(IReconciliationService reconciliationService)
    {
        _reconciliationService = reconciliationService;
    }

    [HttpGet(nameof(GetLitigationRequestForReconciliationAdminReport))]
    public async Task<IActionResult> GetLitigationRequestForReconciliationAdminReport([FromQuery] ReconciliationReportAdminFilterDto filter, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForReconciliationAdminReport(filter, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
}
