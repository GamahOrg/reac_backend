using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Get;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IMobileAppServices.Litigation;
using REAC.Application.Contracts.MobileDtos.Litigation.List;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Mobile.Controllers.Litigation;

[Route("api/mobile/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
//[PermissionRequired(Modules.LitigationRequest)]
[AllowAnonymous]

public class LitigationRequestController : ControllerBase
{
    private readonly IMobileLitigationRequestService _litigationService;
    public LitigationRequestController(IMobileLitigationRequestService litigationService) 
        
    {
        _litigationService = litigationService;
    }
    
    [HttpGet(nameof(List))]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<LitigationListDto>>> List([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey,[FromQuery] List<int> statuses,[FromQuery] List<int> types)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.List(pageIndex, pageSize, searchKey, statuses,types);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationById))]
    [AllowAnonymous]
    public async Task<IActionResult> GetLitigationById(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetById(litigationId);

            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestEvents))]
    public  async Task<IActionResult> GetLitigationRequestEvents(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationRequestEvents(requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    #region Agreement
    [HttpPost(nameof(AddCitizenLitigationRequestForAgreementRequestReply))]
    public async Task<IActionResult> AddCitizenLitigationRequestForAgreementRequestReply([FromBody] AgreementRequestPayload agreementRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.AddCitizenLitigationRequestForAgreementRequestReply(agreementRequestPayload.RequestId, agreementRequestPayload.agreementRequestStatus);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForAgreementRequestReplyByRequestId))]
    public async Task<IActionResult> GetCitizenLitigationRequestForAgreementRequestReplyByRequestId(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetCitizenLitigationRequestForAgreementRequestReply(requestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForAgreementRequestReply))]
    public async Task<IActionResult> GetCitizenLitigationRequestForAgreementRequestReply(int pageIndex,  int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetCitizenLitigationRequestForAgreementRequestReply(pageIndex, pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    #endregion
}