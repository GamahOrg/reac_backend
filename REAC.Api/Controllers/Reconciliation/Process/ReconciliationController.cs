using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.Contracts.IMobileAppServices.Reconciliation;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Api.Controllers.Reconciliation.Process;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Reconciliation)]
[PermissionRequired(Modules.Reconciliation)]
//[AllowAnonymous]

public class ReconciliationController : ControllerBase
{
    private readonly IReconciliationService _reconciliationService;
    private readonly IMobileReconciliationService _mobileReconciliationService;

    public ReconciliationController(IReconciliationService reconciliationService, IMobileReconciliationService mobileReconciliationService)
    {
        _reconciliationService = reconciliationService;
        _mobileReconciliationService = mobileReconciliationService;
    }

    #region Session
    [HttpGet(nameof(GetLitigationRequestForSession))]
    public async Task<IActionResult> GetLitigationRequestForSession(Guid litigationRequestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForSession(litigationRequestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(Assign))]
    public async Task<IActionResult> Assign([FromBody] AssignRequestPayload assignRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.Assign(assignRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpGet(nameof(GetReconciliationRequestForSession))]
    public async Task<IActionResult> GetReconciliationRequestForSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForSession( pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpPost(nameof(AddReconciliationSession))]
    public async Task<IActionResult> AddReconciliationSession([FromBody] ScheduleSessionProcessRequestPayload scheduleSessionProcessRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.AddReconciliationSession(scheduleSessionProcessRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region Report
    [HttpGet(nameof(GetLitigationRequestForReport))]
    public async Task<IActionResult> GetLitigationRequestForReport(Guid litigationRequestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForReport(litigationRequestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetReconciliationRequestForReport))]
    public async Task<IActionResult> GetReconciliationRequestForReport(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForReport(pageIndex,pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(GetLitigationRequestForFullAllReformer))]
    [AllowAnonymous]
    public async Task<IActionResult>  GetLitigationRequestForFullAllReformer(int pageIndex, int pageSize, CancellationToken cancellationToken, LitigationRequestReconciliationAssignReformersFilter? filter, bool ascending = false)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForFullAllReformer(pageIndex, pageSize, cancellationToken,filter, ascending);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForFullAllSession))]
    public async Task<IActionResult> GetLitigationRequestForFullAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForFullAllSession(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForAllSessionForReformer))]
    public async Task<IActionResult> GetLitigationRequestForAllSessionForReformer(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForAllSessionForReformer(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForAllSession))]
    public async Task<IActionResult> GetLitigationRequestForAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestForAllSession(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }



    [HttpPost(nameof(AddReconciliationReport))]
    public async Task<IActionResult> AddReconciliationReport([FromBody] WritingMinutesOfTheMeetingRequestPayload scheduleSessionProcessRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.AddReconciliationReport(scheduleSessionProcessRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    [HttpPost(nameof(AddLitigationRequestDecision))]
    public async Task<IActionResult> AddLitigationRequestDecision([FromBody] LitigationRequestAction litigationRequestAction, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.AddLitigationRequestDecision(litigationRequestAction, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    #region Citizen Replay
    [HttpGet(nameof(AddCitizenLitigationRequestForReportReply))]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportReply(Guid requestId, ApproveRejectStatus minutesOfMeetingApprovalStatus, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            //var returnValue = await _reconciliationService.AddCitizenLitigationRequestForReportReply(requestId, minutesOfMeetingApprovalStatus, cancellationToken);
            var returnValue = await _reconciliationService.CallNafathForReconciliationReport(requestId, minutesOfMeetingApprovalStatus);

            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForReportReply))]
    public async Task<IActionResult> GetCitizenLitigationRequestForReportReply(CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetCitizenLitigationRequestForReportReply(cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForReportReplyByRequestId))]
    public async Task<IActionResult> GetCitizenLitigationRequestForReportReplyByRequestId(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetCitizenLitigationRequestForReportReply(requestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region Extend Period
    [HttpPost(nameof(AddReconciliationExtendPeriod))]
    public async Task<IActionResult> AddReconciliationExtendPeriod([FromBody] LitigationRequestExtendPeriodRequestCreateUpdateDto litigationRequestExtendPeriodRequestCreateUpdateDto, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.AddReconciliationExtendPeriod(litigationRequestExtendPeriodRequestCreateUpdateDto, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestExtendPeriod))]
    public async Task<IActionResult> GetLitigationRequestExtendPeriod(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetPagedLitigationRequestExtendPeriod(pageIndex,pageSize,cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestExtendPeriodByRequestId))]
    public async Task<IActionResult> GetLitigationRequestExtendPeriodByRequestId(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestExtendPeriod(requestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(AddReconciliationExtendPeriodForEmployee))]
    public async Task<IActionResult> AddReconciliationExtendPeriodForEmployee([FromBody] LitigationRequestExtendPeriodRequestCreateUpdateEmployeeDto litigationRequestExtendPeriodRequestCreateUpdateEmployeeDto, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.AddReconciliationExtendPeriodForEmployee(litigationRequestExtendPeriodRequestCreateUpdateEmployeeDto, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestExtendPeriodForEmployee))]
    public async Task<IActionResult> GetLitigationRequestExtendPeriodForEmployee(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestExtendPeriodForEmployee(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestExtendPeriodForEmployeeByRequestId))]
    public async Task<IActionResult> GetLitigationRequestExtendPeriodForEmployeeByRequestId(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _reconciliationService.GetLitigationRequestExtendPeriodForEmployee(requestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion


    #region Mobile 

    #region Reconciliation Report Citizen Replay

    [HttpGet("Mobile/GetCitizenLitigationRequestForReportReply")]
    public async Task<IActionResult> GetCitizenLitigationRequestForReportReplyMobile(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileReconciliationService.GetCitizenLitigationRequestForReportReply(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpPost("Mobile/AddCitizenLitigationRequestForReportReply")]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportReplyMobile(Guid requestId, ApproveRejectStatus minutesOfMeetingApprovalStatus)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileReconciliationService.AddCitizenLitigationRequestForReportReply(requestId, minutesOfMeetingApprovalStatus);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet("Mobile/GetCitizenLitigationRequestForReportReplyByRequestId")]
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

    #endregion


}