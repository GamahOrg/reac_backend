using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;

namespace REAC.Api.Controllers.Arbitration.Process;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Arbitration)]
[PermissionRequired(Modules.Arbitration)]
[AllowAnonymous]

public class ArbitrationReportController : ControllerBase
{
    private readonly IArbitrationService _arbitrationService;

    public ArbitrationReportController(IArbitrationService arbitrationService)
    {
        _arbitrationService = arbitrationService;
    }

    [HttpGet(nameof(GetLitigationRequestForReport))]
    public async Task<IActionResult> GetLitigationRequestForReport(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReport(requestId, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetArbitrationRequestForReport))]
    public async Task<IActionResult> GetArbitrationRequestForReport(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReport(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForReportForAdmin))]
    public async Task<IActionResult> GetLitigationRequestForReportForAdmin([FromQuery] LitigationRequestReportAdminFilterDto filter, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReportForAdmin(filter, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpPost(nameof(AddArbitrationReport))]
    public async Task<IActionResult> AddArbitrationReport([FromBody] WritingArbitrationReportRequestPayload requestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationReport(requestPayload, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(CallNafathForArbitrationReport))]
    public async Task<IActionResult> CallNafathForArbitrationReport(Guid requestId, ApproveRejectStatus agreementRequestStatus)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.CallNafathForArbitrationReport(requestId, agreementRequestStatus);
            return Ok(returnValue);
        }
        return BadRequest();
    }
}
