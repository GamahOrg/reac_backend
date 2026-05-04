using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Arbitration.Process;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Arbitration)]
[PermissionRequired(Modules.Arbitration)]
[AllowAnonymous]

public class ArbitrationCorrectionRequestController : ControllerBase
{
    private readonly IArbitrationService _arbitrationService;

    public ArbitrationCorrectionRequestController(IArbitrationService arbitrationService)
    {
        _arbitrationService = arbitrationService;
    }

    [HttpPost(nameof(AddArbitrationCorrectionRequest))]
    public async Task<IActionResult> AddArbitrationCorrectionRequest(
        [FromBody] LitigationRequestArbitrationCorrectionRequestCreateDto requestPayload,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationCorrectionRequest(requestPayload, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetArbitrationCorrectionRequestsForCitizen))]
    public async Task<IActionResult> GetArbitrationCorrectionRequestsForCitizen(
        int pageIndex, int pageSize,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetArbitrationCorrectionRequestsForCitizen(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetArbitrationCorrectionRequestById))]
    public async Task<IActionResult> GetArbitrationCorrectionRequestById(
        Guid litigationRequestId,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetArbitrationCorrectionRequestById(litigationRequestId, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpPost(nameof(AddArbitrationCorrectionRequestResponse))]
    public async Task<IActionResult> AddArbitrationCorrectionRequestResponse(
        [FromBody] LitigationRequestArbitrationCorrectionRequestResponseDto requestPayload,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationCorrectionRequestResponse(requestPayload, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetArbitrationCorrectionRequestsForArbitrator))]
    public async Task<IActionResult> GetArbitrationCorrectionRequestsForArbitrator(
        int pageIndex, int pageSize,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetArbitrationCorrectionRequestsForArbitrator(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
}
