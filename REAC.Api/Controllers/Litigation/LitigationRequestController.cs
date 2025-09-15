using Core.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Delegator;
using REAC.Application.Contracts.Dtos.Integration.Wafi;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Get;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IMobileAppServices.Litigation;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
//[PermissionRequired(Modules.LitigationRequest)]
//[AllowAnonymous]

public class LitigationRequestController : BaseController<LitigationRequest,
       LitigationCreateUpdateDto, LitigationCreateUpdateDto
       , Guid>
{
    private readonly ILitigationRequestService _litigationService;
    private readonly IMobileLitigationRequestService _mobileLitigationService;

    public LitigationRequestController(ILitigationRequestService litigationRequestService, ILitigationRequestService litigationService, IMobileLitigationRequestService mobileLitigationService) :
        base(litigationRequestService)
    {
        _litigationService = litigationService;
        _mobileLitigationService = mobileLitigationService;
    }
    [HttpPost("CreateLitigation")]
    public override async Task<IActionResult> Create([FromBody] LitigationCreateUpdateDto model,
          CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.Add(model);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet("ApproveArbitrationFeesPaid/{litigationId}")]
    public async Task<IActionResult> ApproveArbitrationFeesPaid(Guid litigationId, CancellationToken cancellationToken)
    {
        return Ok(await _litigationService.ApproveArbitrationFeesPaid(litigationId, cancellationToken));
    }

    [HttpPost("SendLitigationFeesRequestPaid")]
    public async Task<IActionResult> SendLitigationFeesRequestPaid(Guid litigationId,
          CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.SendLitigationFeesRequestPaid(litigationId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost("SendReconciliationFeesRequestPaid")]
    public async Task<IActionResult> SendReconciliationFeesRequestPaid(Guid litigationId,
          CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.SendReconciliationFeesRequestPaid(litigationId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(CreateLitigationRequestManual))]
    public async Task<IActionResult> CreateLitigationRequestManual([FromBody] CreateLitigationRequestNoIntegrationDto model,
          CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            if (model.ContractType == Domain.Shared.Enum.LitigationRequestContractType.Rent)
            {

                var ejarReturnValue = await _litigationService.CreateEjarLitigationRequestManual(model);
                return Ok(ejarReturnValue);

            }
            else if (model.ContractType == Domain.Shared.Enum.LitigationRequestContractType.Properties)
            {

                var ejarReturnValue = await _litigationService.CreatePropertiesLitigationRequestManual(model);
                return Ok(ejarReturnValue);

            }

            var returnValue = await _litigationService.CreateLitigationRequestManual(model);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost("Assign")]
    public async Task<IActionResult> Assign(AssignReviewerRequestPayload assignRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.Assign(assignRequestPayload, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(List))]
    public async Task<ActionResult<IEnumerable<LitigationListDto>>> List([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey, [FromQuery] LitigationListDto model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.List(pageIndex, pageSize, searchKey, model);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(ReformerList))]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<LitigationListDto>>> ReformerList([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey, [FromQuery] LitigationListDto model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.ReformerList(pageIndex, pageSize, searchKey, model);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(AuditngList))]
    //[AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AuditingListDto>>> AuditngList([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey, [FromQuery] AuditingListDto model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.AuditngList(pageIndex, pageSize, searchKey, model);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpGet(nameof(GetReviewersLookup))]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviewersLookup(Guid requestId)

    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetReviewersLookup(requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestForFullAllReviewer))]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<LitigationRequestMircoForRequestReviewerDto>>> GetLitigationRequestForFullAllReviewer(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationRequestForFullAllReviewer(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(AuditngListReconciliation))]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AuditingListDto>>> AuditngListReconciliation([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey, [FromQuery] AuditingListDto model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.AuditngListReconciliation(pageIndex, pageSize, searchKey, model);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationArbitrators))]
    [AllowAnonymous]
    public async Task<IActionResult> GetLitigationArbitrators(Guid requestId)

    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationArbitrators(requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(AuditngListArbitration))]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AuditingListDto>>> AuditngListArbitration([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey, [FromQuery] AuditingListDto model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.AuditngListArbitration(pageIndex, pageSize, searchKey, model);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationById))]
    public async Task<ActionResult<IEnumerable<LitigationRequest>>> GetLitigationById(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.Get(litigationId);
            //if(returnValue.IsDone == false) return Unauthorized(returnValue);

            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationByIdForBackoffice))]
    public async Task<ActionResult<IEnumerable<LitigationRequest>>> GetLitigationByIdForBackoffice(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationByIdForBackoffice(litigationId);

            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpGet(nameof(GetLitigationByIdForEmployee))]
    public async Task<ActionResult<IEnumerable<LitigationRequest>>> GetLitigationByIdForEmployee(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationByIdForEmployee(litigationId);
            if (returnValue.IsDone == false) return Unauthorized(returnValue);

            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationArbitrationTaskAcceptanceOrRejectDocumentData))]
    [AllowAnonymous]
    public async Task<ActionResult<LitigationArbitrationTaskAcceptOrRejectDocumentDto>> GetLitigationArbitrationTaskAcceptanceOrRejectDocumentData(Guid litigationId, Guid? requestId = null)
    {
        if (ModelState.IsValid)
        {
            if (requestId is not null && requestId != Guid.Empty)
            {

                return Ok(await _litigationService.GetLitigationArbitrationTaskAcceptanceOrRejectDocumentData(litigationId, requestId));
            }
            else
            {
                return Ok(await _litigationService.GetLitigationArbitrationTaskAcceptanceOrRejectDocumentData(litigationId));

            }

        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationReconciliationTaskAcceptanceOrRejectDocumentData))]
    [AllowAnonymous]
    public async Task<ActionResult<LitigationReconciliationTaskAcceptOrRejectDocumentDto>> GetLitigationReconciliationTaskAcceptanceOrRejectDocumentData(Guid litigationId, Guid? requestId = null)
    {
        if (ModelState.IsValid)
        {
            if (requestId is not null && requestId != Guid.Empty)
            {

                return Ok(await _litigationService.GetLitigationReconciliationTaskAcceptanceOrRejectDocumentData(litigationId, requestId));
            }
            else
            {
                return Ok(await _litigationService.GetLitigationReconciliationTaskAcceptanceOrRejectDocumentData(litigationId));

            }

        }

        return BadRequest();
    }

    [HttpGet(nameof(GetLitigationPartiesDetailsById))]
    [AllowAnonymous]
    public async Task<ActionResult<LitigationPartiesDetailsDto>> GetLitigationPartiesDetailsById(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationPartiesDetails(litigationId);

            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationDetailsById))]
    [AllowAnonymous]
    public async Task<ActionResult<LitigationDetailsDto>> GetLitigationDetailsById(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationDetails(litigationId);

            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(CreateLitigationRequest))]

    public async Task<ActionResult<LitigationDetailsDto>> CreateLitigationRequest(LitigationRequestDto litigationRequestDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.CreateLitigationRequest(litigationRequestDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet]
    [Route(nameof(GetWafiData))]
    public async Task<DataResponse<WafiDataDto>> GetWafiData(string licenseNumber, string identityNumber)
    {
        return await _litigationService.GetWafiData(licenseNumber, identityNumber);
    }

    #region AgreementRequest
    [HttpPost(nameof(AddCitizenLitigationRequestForAgreementRequestReply))]
    public async Task<IActionResult> AddCitizenLitigationRequestForAgreementRequestReply([FromBody] AgreementRequestPayload agreementRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            //var returnValue = await _litigationService.AddCitizenLitigationRequestForAgreementRequestReply(agreementRequestPayload.RequestId, agreementRequestPayload.agreementRequestStatus);
            var returnValue = await _litigationService.CallNafathForAgreementRequestReply(agreementRequestPayload.RequestId, agreementRequestPayload.agreementRequestStatus);
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
    public async Task<IActionResult> GetCitizenLitigationRequestForAgreementRequestReply()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetCitizenLitigationRequestForAgreementRequestReply();
            return Ok(returnValue);
        }
        return BadRequest();
    }




    #endregion

    #region Delegation

    [HttpGet(nameof(GetLitigationRequestDataForDelegation))]
    public async Task<IActionResult> GetLitigationRequestDataForDelegation(string requestCode, string identityNumber, LitigationRequestParty party)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationRequestDataForDelegation(requestCode,identityNumber,party);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpPost(nameof(AddDelegationRequest))]
    public async Task<IActionResult> AddDelegationRequest(AddDelegationRequest input)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.AddDelegationRequest(input);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetDelegationRequests))]
    public async Task<IActionResult> GetDelegationRequests(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetDelegationRequests(pageIndex,pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetActiveDelegatorsForAdmin))]
    public async Task<IActionResult> GetActiveDelegatorsForAdmin(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetActiveDelegatorsForAdmin(pageIndex,pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(GetLitigationRequestsWithoutDelegatorsForAdmin))]
    public async Task<IActionResult> GetLitigationRequestsWithoutDelegatorsForAdmin(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationRequestsWithoutDelegatorsForAdmin(pageIndex,pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(GetActiveDelegatorsForUser))]
    public async Task<IActionResult> GetActiveDelegatorsForUser(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetActiveDelegatorsForUser(pageIndex,pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(GetLitigationRequestsWithoutDelegatorsForUser))]
    public async Task<IActionResult> GetLitigationRequestsWithoutDelegatorsForUser(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetLitigationRequestsWithoutDelegatorsForUser(pageIndex,pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(GetInActiveDelegatorsForAdmin))]
    public async Task<IActionResult> GetInActiveDelegatorsForAdmin(Guid litigationRequestId, LitigationRequestParty delegationParty)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetInActiveDelegatorsForAdmin(litigationRequestId, delegationParty);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(GetInActiveDelegatorsForUser))]
    public async Task<IActionResult> GetInActiveDelegatorsForUser(Guid litigationRequestId, LitigationRequestParty delegationParty)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetInActiveDelegatorsForUser(litigationRequestId, delegationParty);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(GetDelegationRequestById))]
    public async Task<IActionResult> GetDelegationRequestById(Guid delegationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.GetDelegationRequestById(delegationRequestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(StopDelegator))]
    public async Task<IActionResult> StopDelegator(Guid delegationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.StopDelegator(delegationRequestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(ChangeDelegator))]
    public async Task<IActionResult> ChangeDelegator(Guid delegationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.ChangeDelegator(delegationRequestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    
    [HttpGet(nameof(ApproveRejectDelegationRequest))]
    public async Task<IActionResult> ApproveRejectDelegationRequest(Guid delegationRequestId, bool isApproved, string? reason = null)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationService.ApproveRejectDelegationRequest(delegationRequestId,isApproved,reason);
            return Ok(returnValue);
        }
        return BadRequest();
    }




    #endregion

    #region Mobile

    [HttpGet("Mobile/List")]
    public async Task<ActionResult<IEnumerable<LitigationListDto>>> ListMobile([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey, [FromQuery] List<int> statuses, [FromQuery] List<int> types)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.List(pageIndex, pageSize, searchKey, statuses, types);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet("Mobile/GetLitigationById")]
    public async Task<IActionResult> GetLitigationByIdMobile(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.GetById(litigationId);

            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet("Mobile/GetLitigationRequestEvents")]
    public async Task<IActionResult> GetLitigationRequestEventsMobile(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.GetLitigationRequestEvents(requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    #region Agreement
    [HttpPost("Mobile/AddCitizenLitigationRequestForAgreementRequestReply")]
    public async Task<IActionResult> AddCitizenLitigationRequestForAgreementRequestReplyMobile([FromBody] AgreementRequestPayload agreementRequestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            //var returnValue = await _mobileLitigationService.AddCitizenLitigationRequestForAgreementRequestReply(agreementRequestPayload.RequestId, agreementRequestPayload.agreementRequestStatus);
            var returnValue = await _litigationService.CallNafathForAgreementRequestReply(agreementRequestPayload.RequestId, agreementRequestPayload.agreementRequestStatus);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet("Mobile/GetCitizenLitigationRequestForAgreementRequestReplyByRequestId")]
    public async Task<IActionResult> GetCitizenLitigationRequestForAgreementRequestReplyByRequestIdMobile(Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.GetCitizenLitigationRequestForAgreementRequestReply(requestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet("Mobile/GetCitizenLitigationRequestForAgreementRequestReply")]
    public async Task<IActionResult> GetCitizenLitigationRequestForAgreementRequestReplyMobile(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _mobileLitigationService.GetCitizenLitigationRequestForAgreementRequestReply(pageIndex, pageSize);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    #endregion

    #endregion
    [AllowAnonymous]
    [HttpDelete]
    [Route(nameof(DeleteById))]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var result = await _litigationService.DeleteByIdAsync(id);

        return Ok(result);
    }
}