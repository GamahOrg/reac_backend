using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
[PermissionRequired(Modules.LitigationRequestNote)]

public class LitigationRequestNoteController : BaseController<LitigationRequestNote,
       LitigationRequestNoteCreateUpdateDto, LitigationRequestNoteCreateUpdateDto
       , Guid>
{
    private readonly ILitigationRequestNoteService _litigationNoteService;


    public LitigationRequestNoteController(ILitigationRequestNoteService litigationRequestExtendPeriodRequestService, ILitigationRequestService litigationService, ILitigationRequestNoteService litigationNoteService) :
        base(litigationRequestExtendPeriodRequestService)
    {
        _litigationNoteService = litigationNoteService;
    }

    #region For Reformer / Arbitrator
    [HttpPost("GetLitigationNotes")]
    public async Task<IActionResult> GetLitigationNotes(Guid litigationId, [FromBody] int[] litigationRequestCategoryIds)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationNoteService.GetLitigationNotes(litigationId, litigationRequestCategoryIds);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost("AddLitigationNote")]
    public async Task<IActionResult> AddLitigationNote([FromBody] LitigationRequestNoteCreateUpdateDto model,
          CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationNoteService.AddLitigationNote(model);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost("EditLitigationNote")]
    public async Task<IActionResult> EditLitigationNote([FromBody] LitigationRequestNoteCreateUpdateDto model,
          CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationNoteService.EditLitigationNote(model);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    #endregion

    #region Backoffice

    [HttpGet("GetLitigationRequestNotesForBackoffice")]
    public async Task<IActionResult> GetLitigationRequestNotesForBackoffice(int pageIndex, int pageSize, int litigationRequestCategoryId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationNoteService.GetLitigationRequestNotesForBackoffice(pageIndex,pageSize,litigationRequestCategoryId);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    #endregion

}
