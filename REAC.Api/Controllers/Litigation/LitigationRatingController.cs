using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation.Rating;
using REAC.Application.Contracts.IAppServices;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
//[PermissionRequired(Modules.LitigationRequest)]
[AllowAnonymous]

public class LitigationRatingController : BaseController<LitigationRequest,
       LitigationRatingDto, LitigationRatingDto
       , Guid>
{
    private readonly ILitigationRatingService _litigationRatingService;
    public LitigationRatingController(ILitigationRatingService litigationRequestService, ILitigationRatingService litigationService) :
         base(litigationRequestService)
    {
        _litigationRatingService = litigationService;
    }
    [HttpGet(nameof(GetRatings))]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<LitigationRatingDto>>> GetRatings(Guid litigationId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRatingService.GetRatings(litigationId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(AddRating))]
    [AllowAnonymous]
    public async Task<IActionResult> AddRating(Guid litigationId, Guid citizenId, int ratingValue)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _litigationRatingService.AddRating(litigationId, citizenId, ratingValue);
            return Ok(returnValue);
        }
        return BadRequest();
    }
}
