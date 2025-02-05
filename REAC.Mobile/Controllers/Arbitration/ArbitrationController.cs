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
    private readonly IMobileArbitrationService _mobileArbitrationService;
    public ArbitrationController(IMobileArbitrationService ArbitrationService)
    {
        _mobileArbitrationService = ArbitrationService;
    }

    #region Arbitrators Count
    [HttpPost(nameof(AddArbitratorsCountMobile))]
    public async Task<IActionResult> AddArbitratorsCountMobile(ArbitratorCountRequestPayload arbitratorCountRequestPayload)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.AddArbitratorsCount(arbitratorCountRequestPayload);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpGet(nameof(GetLitigationRequestForAddArbitratorsCountByRequestIdMobile))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorsCountByRequestIdMobile(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetLitigationRequestForAddArbitratorsCount(requestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsCountReplyMobile))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsCountReplyMobile(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetCitizenLitigationRequestForArbitratorsCountReply(pageIndex, pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    #endregion

    #region Nomination List
    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsNominationListReplyByRequestIdMobile))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsNominationListReplyByRequestIdMobile(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetCitizenLitigationRequestForArbitratorsNominationListReply(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(AddCitizenLitigationRequestForReportArbitratorsNominationListReplyMobile))]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportArbitratorsNominationListReplyMobile([FromBody] ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.AddCitizenLitigationRequestForReportArbitratorsNominationListReply(arbitratorNominationListRequestReplyPayload);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsNominationListReplyMobile))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsNominationListReplyMobile(int pageIndex, int pageSize ,CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetCitizenLitigationRequestForArbitratorsNominationListReply(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region Arbitration Verdict

    [HttpGet(nameof(GetLitigationRequestForReportByRequestIdMobile))]
    public async Task<IActionResult> GetLitigationRequestForReportByRequestIdMobile(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetLitigationRequestForReport(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region Explanation Request

    [HttpGet(nameof(GetLitigationRequestForReportExplanationRequestMobile))]
    public async Task<IActionResult> GetLitigationRequestForReportExplanationRequestMobile(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetLitigationRequestForReportExplanationRequest(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpPost(nameof(AddArbitrationReportExplanationRequestMobile))]
    public async Task<IActionResult> AddArbitrationReportExplanationRequestMobile([FromBody] LitigationRequestArbitrationReportExplanationRequestCreateDto litigationRequestArbitrationReportExplanationRequestCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.AddArbitrationReportExplanationRequest(litigationRequestArbitrationReportExplanationRequestCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpGet(nameof(GetLitigationRequestForReportExplanationRequestByRequestIdMobile))]
    public async Task<IActionResult> GetLitigationRequestForReportExplanationRequestByRequestIdMobile(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetLitigationRequestForReportExplanationRequest(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForReportExplanationRequestForArbitratorByRequestIdMobile))]
    public async Task<IActionResult> GetLitigationRequestForReportExplanationRequestForArbitratorByRequestIdMobile(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetLitigationRequestForReportExplanationRequestForArbitrator(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForReportExplanationRequestForArbitratorMobile))]
    public async Task<IActionResult> GetLitigationRequestForReportExplanationRequestForArbitratorMobile(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileArbitrationService.GetLitigationRequestForReportExplanationRequestForArbitrator(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    #endregion
}