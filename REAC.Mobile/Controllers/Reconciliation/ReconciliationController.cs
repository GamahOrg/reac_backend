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
    private readonly IMobileReconciliationService _mobileReconciliationService;
    public ReconciliationController(IMobileReconciliationService reconciliationService)
    {
        _mobileReconciliationService = reconciliationService;
    }

    #region Reconciliation Report Citizen Replay

    [HttpGet(nameof(GetCitizenLitigationRequestForReportReplyMobile))]
    public async Task<IActionResult> GetCitizenLitigationRequestForReportReplyMobile(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileReconciliationService.GetCitizenLitigationRequestForReportReply( pageIndex,  pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpPost(nameof(AddCitizenLitigationRequestForReportReplyMobile))]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportReplyMobile(Guid requestId, ApproveRejectStatus minutesOfMeetingApprovalStatus)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileReconciliationService.AddCitizenLitigationRequestForReportReply(requestId, minutesOfMeetingApprovalStatus);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForReportReplyByRequestIdMobile))]
    public async Task<IActionResult> GetCitizenLitigationRequestForReportReplyByRequestIdMobile(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileReconciliationService.GetCitizenLitigationRequestForReportReply(requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion




}