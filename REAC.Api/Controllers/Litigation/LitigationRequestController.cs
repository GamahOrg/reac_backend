using Core.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using REAC.EntityFrameworkCore.Entities.Litigation;

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
 [HttpGet("GetVersion")]
 public async Task<IActionResult> GetVersion(Guid litigationId, CancellationToken cancellationToken)
 {
     return Ok(1);
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
            var returnValue = await _mobileLitigationService.AddCitizenLitigationRequestForAgreementRequestReply(agreementRequestPayload.RequestId, agreementRequestPayload.agreementRequestStatus);
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
}
