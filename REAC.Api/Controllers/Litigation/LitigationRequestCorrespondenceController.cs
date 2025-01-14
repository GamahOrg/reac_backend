using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
//[PermissionRequired(Modules.LitigationRequest)]

public class LitigationRequestCorrespondenceController : BaseController<LitigationRequestCorrespondence,
       LitigationRequestCorrespondenceCreateUpdateDto, LitigationRequestCorrespondenceCreateUpdateDto
       , Guid>
{
    private readonly ILitigationRequestCorrespondenceService _litigationRequestCorrespondenceService;
    public LitigationRequestCorrespondenceController(ILitigationRequestCorrespondenceService litigationRequestCorrespondenceService) :
        base(litigationRequestCorrespondenceService)
    {
        _litigationRequestCorrespondenceService = litigationRequestCorrespondenceService;
    }

    #region For Reformer / Arbitrator

    [HttpPost("GetLitigationCorrespondences")]
    public async Task<IActionResult> GetLitigationCorrespondences(Guid litigationId, [FromBody] int[] litigationRequestCategoryIds)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestCorrespondenceService.GetLitigationCorrespondences(litigationId, litigationRequestCategoryIds);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost("AddLitigationCorrespondence")]
    public async Task<IActionResult> AddLitigationCorrespondence([FromBody] LitigationRequestCorrespondenceCreateUpdateDto model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestCorrespondenceService.AddLitigationCorrespondence(model);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    #endregion


    #region Correspondences Reply For Defendant/plaintiff

    [HttpGet("GetLitigationCorrespondenceReply")]
    public async Task<IActionResult> GetLitigationCorrespondenceReply([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey, [FromQuery] LitigationListDto model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestCorrespondenceService.GetLitigationRequestsForCitizinCorrespondenceReply(pageIndex,pageSize,searchKey,model);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    
    [HttpGet("GetLitigationCorrespondencesByLitigationRequestId")]
    public async Task<IActionResult> GetLitigationCorrespondencesByLitigationRequestId(Guid litigationRequestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestCorrespondenceService.GetLitigationCorrespondencesByLitigationRequestId(litigationRequestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost("AddLitigationCorrespondenceReply")]
    public async Task<IActionResult> AddLitigationCorrespondenceReply([FromBody] LitigationRequestCorrespondenceReplyCreateUpdateDto model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestCorrespondenceService.AddLitigationCorrespondenceReply(model);
            return Ok(returnValue);
        }

        return BadRequest();
    }



    #endregion


    #region Backoffice
    [HttpGet(nameof(GetLitigationRequestArbitrationCorrespondence))]
    public async Task<IActionResult> GetLitigationRequestArbitrationCorrespondence(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestCorrespondenceService.GetLitigationRequestArbitrationCorrespondence(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    
    [HttpGet(nameof(GetLitigationRequestReconciliationCorrespondence))]
    public async Task<IActionResult> GetLitigationRequestReconciliationCorrespondence(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRequestCorrespondenceService.GetLitigationRequestReconciliationCorrespondence(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    #endregion

}