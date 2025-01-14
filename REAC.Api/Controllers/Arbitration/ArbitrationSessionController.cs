using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Arbitration.Process;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Arbitration)]
[PermissionRequired(Modules.ArbitrationSession)]
[AllowAnonymous]

public class ArbitrationSessionController : ControllerBase
{
    private readonly IArbitrationService _ArbitrationService;
    public ArbitrationSessionController(IArbitrationService ArbitrationService)
    {
        _ArbitrationService = ArbitrationService;
    }

    #region Session
    [HttpGet(nameof(GetLitigationRequestForSession))]
    public async Task<IActionResult> GetLitigationRequestForSession(Guid litigationRequestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _ArbitrationService.GetLitigationRequestForSession(litigationRequestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetArbitrationRequestForSession))]
    public async Task<IActionResult> GetArbitrationRequestForSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _ArbitrationService.GetLitigationRequestForSession(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpPost(nameof(AddArbitrationSession))]
    public async Task<IActionResult> AddArbitrationSession([FromBody] ScheduleSessionProcessRequestPayload scheduleSessionProcessRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _ArbitrationService.AddArbitrationSession(scheduleSessionProcessRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForFullAllSession))]
    public async Task<IActionResult> GetLitigationRequestForFullAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _ArbitrationService.GetLitigationRequestForFullAllSession(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForAllSessionForArbitrator))]
    public async Task<IActionResult> GetLitigationRequestForAllSessionForArbitrator(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _ArbitrationService.GetLitigationRequestForAllSessionForArbitrator(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForAllSession))]
    public async Task<IActionResult> GetLitigationRequestForAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _ArbitrationService.GetLitigationRequestForAllSession(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }


    #endregion



}