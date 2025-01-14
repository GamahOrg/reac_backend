using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Reconciliation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Arbitration)]
[PermissionRequired(Modules.LitigationRequestArbitratorAssigningRequest)]

public class LitigationRequestArbitratorAssigningRequestController : ControllerBase
{
    private readonly ILitigationRequestArbitratorAssigningRequestService _litigationRequestArbitratorAssigningRequest;
    public LitigationRequestArbitratorAssigningRequestController(ILitigationRequestArbitratorAssigningRequestService litigationRequestArbitratorAssigningRequest)
    {
        _litigationRequestArbitratorAssigningRequest = litigationRequestArbitratorAssigningRequest;
    }
    [HttpGet(nameof(GetLitigationRequestArbitratorAssigningRequests))]
    public async Task<IActionResult> GetLitigationRequestArbitratorAssigningRequests(CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestArbitratorAssigningRequest.GetLitigationRequestArbitratorAssigningRequests(cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(AddLitigationRequestArbitratorAssigningRequestsResponse))]
    public async Task<IActionResult> AddLitigationRequestArbitratorAssigningRequestsResponse([FromBody] AssigningArbitratorRequestsPayload requestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestArbitratorAssigningRequest.AddLitigationRequestArbitratorAssigningRequestsResponse(requestPayload, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestArbitratorAssigningRequestsByRequestId))]
    public async Task<IActionResult> GetLitigationRequestArbitratorAssigningRequestsByRequestId(Guid requestId, Guid LitigationRequestArbitratorAssigningRequestId, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestArbitratorAssigningRequest.GetLitigationRequestArbitratorAssigningRequests(requestId, LitigationRequestArbitratorAssigningRequestId, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestArbitratorAssigningRequestsAll))]
    public async Task<IActionResult> GetLitigationRequestArbitratorAssigningRequestsAll(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestArbitratorAssigningRequest.GetLitigationRequestArbitratorAssigningRequestsAll(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }

}

