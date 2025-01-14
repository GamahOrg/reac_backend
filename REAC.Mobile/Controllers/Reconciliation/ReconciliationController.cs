using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IMobileAppServices.Reconciliation;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;

namespace REAC.Mobile.Controllers.Reconciliation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Reconciliation)]
[PermissionRequired(Modules.Reconciliation)]
[AllowAnonymous]

public class ReconciliationController : ControllerBase
{
    private readonly IMobileReconciliationService _reconciliationService;
    public ReconciliationController(IMobileReconciliationService reconciliationService)
    {
        _reconciliationService = reconciliationService;
    }

    #region Reconciliation Report Citizen Replay

    [HttpGet(nameof(GetCitizenLitigationRequestForReportReply))]
    public async Task<IActionResult> GetCitizenLitigationRequestForReportReply(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetCitizenLitigationRequestForReportReply( pageIndex,  pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpPost(nameof(AddCitizenLitigationRequestForReportReply))]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportReply(Guid requestId, ApproveRejectStatus minutesOfMeetingApprovalStatus)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.AddCitizenLitigationRequestForReportReply(requestId, minutesOfMeetingApprovalStatus);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForReportReplyByRequestId))]
    public async Task<IActionResult> GetCitizenLitigationRequestForReportReplyByRequestId(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetCitizenLitigationRequestForReportReply(requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion




}