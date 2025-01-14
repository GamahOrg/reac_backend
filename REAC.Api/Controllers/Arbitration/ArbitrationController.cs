using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Api.Controllers.Arbitration.Process;

public class AddChairmainRequestPayload
{
    public Guid RequestId { get; set; }

    public Guid ArbitratorChairmanId { get; set; }
}

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Arbitration)]
[PermissionRequired(Modules.Arbitration)]
[AllowAnonymous]

public class ArbitrationController : ControllerBase
{
    private readonly IArbitrationService _arbitrationService;
    public ArbitrationController(IArbitrationService ArbitrationService)
    {
        _arbitrationService = ArbitrationService;
    }

    #region ArbitratorSelectionDuration
    [HttpPost(nameof(AddArbitratorSelectionDuration))]
    public async Task<IActionResult> AddArbitratorSelectionDuration(ArbitratorSelectionDurationRequestPayload arbitratorSelectionDurationRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitratorSelectionDuration(arbitratorSelectionDurationRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForAddArbitratorSelectionDuration))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorSelectionDuration(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForAddArbitratorSelectionDuration(requestId, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForArbitratorSelectionDuration))]
    public async Task<IActionResult> GetLitigationRequestForArbitratorSelectionDuration(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForArbitratorSelectionDuration(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    #endregion

    #region ArbitratorsCount
    [HttpPost(nameof(AddArbitratorsCount))]
    public async Task<IActionResult> AddArbitratorsCount(ArbitratorCountRequestPayload arbitratorCountRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitratorsCount(arbitratorCountRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForAddArbitratorsCount))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorsCount(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForAddArbitratorsCount(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForAddArbitratorsCountByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorsCountByRequestId(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForAddArbitratorsCount(requestId, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpPost(nameof(AddCitizenLitigationRequestForReportArbitratorsCountReply))]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportArbitratorsCountReply(ArbitratorCountRequestPayload arbitratorCountRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddCitizenLitigationRequestForReportArbitratorsCountReply(arbitratorCountRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsCountReply))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsCountReply(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsCountReply(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsCountReplyByRequestId))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsCountReplyByRequestId(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsCountReply(requestId, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    #endregion

    #region ExtendPeriod
    [HttpPost(nameof(AddArbitrationExtendPeriod))]
    public async Task<IActionResult> AddArbitrationExtendPeriod([FromBody] LitigationRequestExtendPeriodRequestCreateUpdateDto litigationRequestExtendPeriodRequestCreateUpdateDto, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationExtendPeriod(litigationRequestExtendPeriodRequestCreateUpdateDto, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpPost(nameof(AddArbitrationExtendPeriodForEmployee))]
    public async Task<IActionResult> AddArbitrationExtendPeriodForEmployee([FromBody] LitigationRequestExtendPeriodRequestCreateUpdateEmployeeDto litigationRequestExtendPeriodRequestCreateUpdateEmployeeDto, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationExtendPeriodForEmployee(litigationRequestExtendPeriodRequestCreateUpdateEmployeeDto, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestExtendPeriodForEmployeeByrequestId))]
    public async Task<IActionResult> GetLitigationRequestExtendPeriodForEmployeeByrequestId(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestExtendPeriodForEmployee(requestId, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestExtendPeriodByrequestId))]
    public async Task<IActionResult> GetLitigationRequestExtendPeriodByrequestId(Guid requestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestExtendPeriod(requestId, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestExtendPeriod))]
    public async Task<IActionResult> GetLitigationRequestExtendPeriod(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestExtendPeriod(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestExtendPeriodForEmployee))]
    public async Task<IActionResult> GetLitigationRequestExtendPeriodForEmployee(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestExtendPeriodForEmployee(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    #endregion

    #region ArbitrationReport
    [HttpPost(nameof(AddArbitrationReport))]
    public async Task<IActionResult> AddArbitrationReport([FromBody] WritingArbitrationReportRequestPayload writingArbitrationReportRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationReport(writingArbitrationReportRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForReportByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForReportByRequestId(Guid litigationRequestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReport(litigationRequestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForReport))]
    public async Task<IActionResult> GetLitigationRequestForReport(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForReport(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region ExplanationRequest
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

    [HttpPost(nameof(AddArbitrationReportExplanationRequestForArbitrator))]
    public async Task<IActionResult> AddArbitrationReportExplanationRequestForArbitrator([FromBody] LitigationRequestArbitrationReportExplanationResponseDto litigationRequestArbitrationReportExplanationResponseDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationReportExplanationRequestForArbitrator(litigationRequestArbitrationReportExplanationResponseDto);
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

    #region NominationListGetCitizenLitigationRequestForArbitratorsNominationListReplyByRequestId
    [HttpGet(nameof(GetLitigationRequestForAddArbitratorsNominationList))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorsNominationList(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForAddArbitratorsNominationList(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForAddArbitratorsNominationListByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorsNominationListByRequestId(Guid litigationRequestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForAddArbitratorsNominationList(litigationRequestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(AddLitigationRequestArbitratorsNominationList))]
    public async Task<IActionResult> AddLitigationRequestArbitratorsNominationList([FromBody] ArbitratorNominationListRequestPayload arbitratorNominationListRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddLitigationRequestArbitratorsNominationList(arbitratorNominationListRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsNominationListReply))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsNominationListReply(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsNominationListReply(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsNominationListReplyByRequestId))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsNominationListReplyByRequestId(Guid litigationRequestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsNominationListReply(litigationRequestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(AddCitizenLitigationRequestForReportArbitratorsNominationListReply))]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportArbitratorsNominationListReply([FromBody] ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddCitizenLitigationRequestForReportArbitratorsNominationListReply(arbitratorNominationListRequestReplyPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region Chairman NominationList
    [HttpGet(nameof(GetLitigationRequestForAddArbitratorsChairmanNominationList))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorsChairmanNominationList(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForAddArbitratorsChairmanNominationList(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForAddArbitratorsChairmanNominationListByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForAddArbitratorsChairmanNominationListByRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForAddArbitratorsChairmanNominationList(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(AddLitigationRequestArbitratorsChairmanNominationList))]
    public async Task<IActionResult> AddLitigationRequestArbitratorsChairmanNominationList([FromBody] ArbitratorNominationListRequestPayload arbitratorNominationListRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddLitigationRequestArbitratorsChairmanNominationList(arbitratorNominationListRequestPayload);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(AddLitigationRequestArbitratorsChairmandecision))]
    public async Task<IActionResult> AddLitigationRequestArbitratorsChairmandecision([FromBody] AddChairmainRequestPayload addChairmainRequestPayload)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddLitigationRequestArbitratorsChairmandecision(addChairmainRequestPayload.RequestId, addChairmainRequestPayload.ArbitratorChairmanId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetCitizenLitigationRequestForArbitratorsChairmanNominationListReplyByRequestId))]
    public async Task<IActionResult> GetCitizenLitigationRequestForArbitratorsChairmanNominationListReplyByRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(AddCitizenLitigationRequestForReportArbitratorsChairmanNominationListReply))]
    public async Task<IActionResult> AddCitizenLitigationRequestForReportArbitratorsChairmanNominationListReply([FromBody] ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddCitizenLitigationRequestForReportArbitratorsChairmanNominationListReply(arbitratorNominationListRequestReplyPayload);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

    #region RecusalRequest
    [HttpPost(nameof(AddArbitrationArbitratorRecusalRequest))]
    public async Task<IActionResult> AddArbitrationArbitratorRecusalRequest([FromBody] LitigationRequestArbitrationArbitratorRecusalRequestCreateDto litigationRequestArbitrationArbitratorRecusalRequestCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationArbitratorRecusalRequest(litigationRequestArbitrationArbitratorRecusalRequestCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    // Portal
    [HttpGet(nameof(GetLitigationRequestForArbitratorRecusalRequestByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForArbitratorRecusalRequestByRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForArbitratorRecusalRequest(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForArbitratorRecusalRequest))]
    public async Task<IActionResult> GetLitigationRequestForArbitratorRecusalRequest(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForArbitratorRecusalRequest(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    // Backoffice
    [HttpPost(nameof(AddArbitrationReportArbitratorRecusalForArbitrator))]
    public async Task<IActionResult> AddArbitrationReportArbitratorRecusalForArbitrator([FromBody] LitigationRequestArbitrationRecusalRequestResponseDto litigationRequestArbitrationRecusalRequestResponseDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.AddArbitrationReportArbitratorRecusalForArbitrator(litigationRequestArbitrationRecusalRequestResponseDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    
    [HttpGet(nameof(GetLitigationRequestForArbitratorRecusalRequestForArbitratorByRequestId))]
    public async Task<IActionResult> GetLitigationRequestForArbitratorRecusalRequestForArbitratorByRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForArbitratorRecusalRequestForArbitratorByRequestId(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestForArbitratorRecusalRequestForArbitrator))]
    public async Task<IActionResult> GetLitigationRequestForArbitratorRecusalRequestForArbitrator(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetLitigationRequestForArbitratorRecusalRequestForArbitrator(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }



    #endregion


    [HttpPost(nameof(Assign))]
    public async Task<IActionResult> Assign([FromBody] AssignArbitratorRequestPayload assignArbitratorRequestPayload)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.Assign(assignArbitratorRequestPayload);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetAllArbitrationLitigationRequest))]
    public async Task<IActionResult> GetAllArbitrationLitigationRequest(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _arbitrationService.GetAllArbitrationLitigationRequest(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }


}