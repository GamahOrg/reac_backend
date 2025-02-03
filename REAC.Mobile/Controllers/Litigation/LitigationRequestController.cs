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
    private readonly IMobileLitigationRequestService _mobileLitigationService;
    public LitigationRequestController(IMobileLitigationRequestService litigationService) 
        
    {
        _mobileLitigationService = litigationService;
    }
    
    [HttpGet(nameof(ListMobile))]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<LitigationListDto>>> ListMobile([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey,[FromQuery] List<int> statuses,[FromQuery] List<int> types)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.List(pageIndex, pageSize, searchKey, statuses,types);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationByIdMobile))]
    [AllowAnonymous]
    public async Task<IActionResult> GetLitigationByIdMobile(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.GetById(litigationId);

            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationRequestEventsMobile))]
    public  async Task<IActionResult> GetLitigationRequestEventsMobile(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.GetLitigationRequestEvents(requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    #region Agreement
    [HttpPost(nameof(AddCitizenLitigationRequestForAgreementRequestReplyMobile))]
    public async Task<IActionResult> AddCitizenLitigationRequestForAgreementRequestReplyMobile([FromBody] AgreementRequestPayload agreementRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.AddCitizenLitigationRequestForAgreementRequestReply(agreementRequestPayload.RequestId, agreementRequestPayload.agreementRequestStatus);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForAgreementRequestReplyByRequestIdMobile))]
    public async Task<IActionResult> GetCitizenLitigationRequestForAgreementRequestReplyByRequestIdMobile(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.GetCitizenLitigationRequestForAgreementRequestReply(requestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetCitizenLitigationRequestForAgreementRequestReplyMobile))]
    public async Task<IActionResult> GetCitizenLitigationRequestForAgreementRequestReplyMobile(int pageIndex,  int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.GetCitizenLitigationRequestForAgreementRequestReply(pageIndex, pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    #endregion
}