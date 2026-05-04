using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation.Rating;
using REAC.Application.Contracts.IAppServices;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
[AllowAnonymous]
public class BeneficiarySatisfactionSurveyController : BaseController<BeneficiarySatisfactionSurveyDto,
    BeneficiarySatisfactionSurveyDto, BeneficiarySatisfactionSurveyDto, Guid>
{
    private readonly IBeneficiarySatisfactionSurveyService _surveyService;

    public BeneficiarySatisfactionSurveyController(IBeneficiarySatisfactionSurveyService surveyService)
        : base(surveyService)
    {
        _surveyService = surveyService;
    }

    [HttpPost(nameof(AddSurvey))]
    public async Task<IActionResult> AddSurvey([FromBody] BeneficiarySatisfactionSurveyDto dto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _surveyService.AddSurvey(dto);
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(HasUserRated))]
    public async Task<IActionResult> HasUserRated(Guid litigationRequestId, Guid citizenId, int stageType)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _surveyService.HasUserRated(litigationRequestId, citizenId, stageType);
            return Ok(returnValue);
        }
        return BadRequest();
    }
}
