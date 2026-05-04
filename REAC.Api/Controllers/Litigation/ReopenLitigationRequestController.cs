using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
[PermissionRequired(Modules.LitigationRequest)]
[AllowAnonymous]

public class ReopenLitigationRequestController : ControllerBase
{
    private readonly ILitigationRequestAdminService _adminService;

    public ReopenLitigationRequestController(ILitigationRequestAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet(nameof(GetClosedLitigationRequests))]
    public async Task<IActionResult> GetClosedLitigationRequests(
        int pageIndex, int pageSize,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _adminService.GetClosedLitigationRequests(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestReopenInfo))]
    public async Task<IActionResult> GetLitigationRequestReopenInfo(
        Guid litigationRequestId,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _adminService.GetLitigationRequestReopenInfo(litigationRequestId, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpPost(nameof(ReopenLitigationRequest))]
    public async Task<IActionResult> ReopenLitigationRequest(
        [FromBody] ReopenLitigationRequestDto requestPayload,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _adminService.ReopenLitigationRequest(requestPayload, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
}
