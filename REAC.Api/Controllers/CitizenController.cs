using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.IAppServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Public)]
[PermissionRequired(Modules.Citizen)]

public class CitizenController : ControllerBase
{
    private readonly ICitizenBankAccountService _citizenBankAccountService;

    private readonly ICitizenService _citizenService;
    public CitizenController(ICitizenService citizenService, ICitizenBankAccountService citizenBankAccountService)
    {
        _citizenService = citizenService;
        _citizenBankAccountService = citizenBankAccountService;
    }

    [HttpGet(nameof(GetCitizenBasicData))]
    public async Task<ActionResult> GetCitizenBasicData()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetCitizenBasicData();
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(UpdateCitizenBasicData))]
    public async Task<ActionResult> UpdateCitizenBasicData(SaveCitizenBasicDataProfileDto citizenBasicDataProfileDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.UpdateCitizenBasicData(citizenBasicDataProfileDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(UpdateCitizenBankAccount))]
    public async Task<ActionResult> UpdateCitizenBankAccount(CitizenBankAccountCreateUpdateDto citizenBankAccountCreateUpdateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenBankAccountService.UpdateCitizenBankAccount(citizenBankAccountCreateUpdateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(AddArbitratorFreeUpDecision))]
    public async Task<ActionResult> AddArbitratorFreeUpDecision(ArbitratorFreeUpDecisionCreateDto arbitratorFreeUpDecisionCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.AddArbitratorFreeUpDecision(arbitratorFreeUpDecisionCreateDto);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpPost(nameof(AddArbitratorStopDecision))]
    public async Task<ActionResult> AddArbitratorStopDecision(ArbitratorStopDecisionCreateDto arbitratorStopDecisionCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.AddArbitratorStopDecision(arbitratorStopDecisionCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(AddReformerFreeUpDecision))]
    public async Task<ActionResult> AddReformerFreeUpDecision(ReformerFreeUpDecisionCreateDto reformerFreeUpDecisionCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.AddReformerFreeUpDecision(reformerFreeUpDecisionCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(AddReformerStopDecision))]
    public async Task<ActionResult> AddReformerStopDecision(ReformerStopDecisionCreateDto reformerStopDecisionCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.AddReformerStopDecision(reformerStopDecisionCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    
    [HttpPost(nameof(ActivateReformerAccount))]
    public async Task<ActionResult> ActivateReformerAccount(ReformerFreeUpDecisionCreateDto reformerFreeUpDecisionCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.ActivateReformerAccount(reformerFreeUpDecisionCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    
    [HttpPost(nameof(ActivateArbitratorAccount))]
    public async Task<ActionResult> ActivateArbitratorAccount(ArbitratorFreeUpDecisionCreateDto reformerFreeUpDecisionCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.ActivateArbitratorAccount(reformerFreeUpDecisionCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(DeActivateReformerAccount))]
    public async Task<ActionResult> DeActivateReformerAccount(ReformerStopDecisionCreateDto reformerStopDecisionCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.DeActivateReformerAccount(reformerStopDecisionCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    
    [HttpPost(nameof(DeActivateArbitratorAccount))]
    public async Task<ActionResult> DeActivateArbitratorAccount(ArbitratorStopDecisionCreateDto reformerStopDecisionCreateDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.DeActivateArbitratorAccount(reformerStopDecisionCreateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }


    [HttpGet(nameof(GetCitizenBankAccount))]
    public async Task<ActionResult> GetCitizenBankAccount()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenBankAccountService.GetCitizenBankAccount();
            return Ok(returnValue);
        }

        return BadRequest();
    }

    // محامى عن شخص
    [HttpGet(nameof(GetDelegatedPersonsLawyer))]
    public async Task<ActionResult> GetDelegatedPersonsLawyer()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetDelegatedPersonsLawyer();
            return Ok(returnValue);
        }

        return BadRequest();
    }
    // ممثل عن شخص
    [HttpGet(nameof(GetDelegatedPersonsAgent))]
    public async Task<ActionResult> GetDelegatedPersonsAgent()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetDelegatedPersonsAgent();
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetReformersLookup))]
    public async Task<ActionResult> GetReformersLookup(Guid? citizenId, Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetReformersLookup(citizenId, requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetArbitratorsLookup))]
    public async Task<ActionResult> GetArbitratorsLookup(Guid? citizenId, Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetArbitratorsLookup(citizenId, requestId);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpGet(nameof(GetArbitratorsFullLookup))]
    public async Task<ActionResult> GetArbitratorsFullLookup(Guid? citizenId, Guid requestId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetArbitratorsFullLookup(citizenId, requestId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetReformersDecisions))]
    public async Task<ActionResult> GetReformersDecisions(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetReformersDecisions(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetReformersDecisionsByCitizenId))]
    public async Task<ActionResult> GetReformersDecisionsByCitizenId(Guid citizenId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetReformersDecisions(citizenId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetArbitratorsDecisions))]
    public async Task<ActionResult> GetArbitratorsDecisions(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetArbitratorsDecisions(pageIndex, pageSize, cancellationToken);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetArbitratorsDecisionsByCitizenId))]
    public async Task<ActionResult> GetArbitratorsDecisionsByCitizenId(Guid citizenId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetArbitratorsDecisions(citizenId);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    // محامى عن شركة
    [HttpGet(nameof(GetDelegatedFacilitiesLawyer))]
    public async Task<ActionResult> GetDelegatedFacilitiesLawyer()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetDelegatedFacilitiesLawyer();
            return Ok(returnValue);
        }
        return BadRequest();
    }

    [HttpGet(nameof(GetFacilities))]
    public async Task<ActionResult> GetFacilities(RelationShipType relationShipId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetFacilities(relationShipId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet(nameof(GetPersons))]
    public async Task<ActionResult> GetPersons(RelationShipType relationShipId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetPersons(relationShipId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    // وكيل عن شركة
    [HttpGet(nameof(GetDelegatedFacilitiesAgent))]
    public async Task<ActionResult> GetDelegatedFacilitiesAgent()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetDelegatedFacilitiesAgent();
            return Ok(returnValue);
        }

        return BadRequest();
    }

    // ممثل عن شركة
    [HttpGet(nameof(GetOwnerFacilities))]
    public async Task<ActionResult> GetOwnerFacilities()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenService.GetOwnerFacilities();
            return Ok(returnValue);
        }

        return BadRequest();
    }
}