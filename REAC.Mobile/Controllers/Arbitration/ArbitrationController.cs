using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.Contracts.IMobileAppServices.Arbitration;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Mobile.Controllers.Arbitration.Process;

[Route("api/mobile/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Arbitration)]
[PermissionRequired(Modules.Arbitration)]
[AllowAnonymous]

public class ArbitrationController : ControllerBase
{
    private readonly IMobileArbitrationService _arbitrationService;
    public ArbitrationController(IMobileArbitrationService ArbitrationService)
    {
        _arbitrationService = ArbitrationService;
    }

    #region Arbitrators Count
    [HttpPost(nameof(AddArbitratorsCount))]
    public async Task<IActionResult> AddArbitratorsCount(ArbitratorCountRequestPayload arbitratorCountRequestPayload)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitratorsCount(arbitratorCountRequestPayload);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpGet(nameof(GetLitigationRequestForAddArbitratorsCountByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorsCountByRequestId(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForAddArbitratorsCount(requestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsCountReply))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsCountReply(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsCountReply(pageIndex, pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    #endregion

    #region Nomination List
    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsNominationListReplyByRequestId))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsNominationListReplyByRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsNominationListReply(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(AddCitizenLitigationRequestForReportArbitratorsNominationListReply))]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportArbitratorsNominationListReply([FromBody] ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddCitizenLitigationRequestForReportArbitratorsNominationListReply(arbitratorNominationListRequestReplyPayload);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsNominationListReply))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsNominationListReply(int pageIndex, int pageSize ,CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsNominationListReply(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region Arbitration Verdict

    [HttpGet(nameof(GetLitigationRequestForReportByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForReportByRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReport(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region Explanation Request

    [HttpGet(nameof(GetLitigationRequestForReportExplanationRequest))]
    public async Task<IActionResult> GetLitigationRequestForReportExplanationRequest(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReportExplanationRequest(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpPost(nameof(AddArbitrationReportExplanationRequest))]
    public async Task<IActionResult> AddArbitrationReportExplanationRequest([FromBody] LitigationRequestArbitrationReportExplanationRequestCreateDto litigationRequestArbitrationReportExplanationRequestCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationReportExplanationRequest(litigationRequestArbitrationReportExplanationRequestCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpGet(nameof(GetLitigationRequestForReportExplanationRequestByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForReportExplanationRequestByRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReportExplanationRequest(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForReportExplanationRequestForArbitratorByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForReportExplanationRequestForArbitratorByRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReportExplanationRequestForArbitrator(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForReportExplanationRequestForArbitrator))]
    public async Task<IActionResult> GetLitigationRequestForReportExplanationRequestForArbitrator(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReportExplanationRequestForArbitrator(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    #endregion
}