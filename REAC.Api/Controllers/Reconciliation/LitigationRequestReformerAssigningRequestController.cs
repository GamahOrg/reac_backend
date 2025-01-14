using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Reconciliation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Reconciliation)]
[PermissionRequired(Modules.LitigationRequestReformerAssigningRequest)]

public class LitigationRequestReformerAssigningRequestController : ControllerBase
{
    private readonly ILitigationRequestReformerAssigningRequest _litigationRequestReformerAssigningRequest;
    public LitigationRequestReformerAssigningRequestController(ILitigationRequestReformerAssigningRequest litigationRequestReformerAssigningRequest)
    {
        _litigationRequestReformerAssigningRequest = litigationRequestReformerAssigningRequest;
    }

    [HttpGet(nameof(GetLitigationRequestReformerAssigningRequests))]
    public async Task<IActionResult> GetLitigationRequestReformerAssigningRequests(CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestReformerAssigningRequest.GetLitigationRequestReformerAssigningRequests(cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetLitigationRequestReformerAssigningRequestsAll))]
    public async Task<IActionResult> GetLitigationRequestReformerAssigningRequestsAll(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestReformerAssigningRequest.GetLitigationRequestReformerAssigningRequestsAll(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(AddLitigationRequestReformerAssigningRequestsResponse))]
    public async Task<IActionResult> AddLitigationRequestReformerAssigningRequestsResponse([FromBody] AssigningRequestsPayload requestPayload, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestReformerAssigningRequest.AddLitigationRequestReformerAssigningRequestsResponse(requestPayload, cancellationToken);
            return Ok(returnValue);
        }
        return BadRequest();
    }

}

