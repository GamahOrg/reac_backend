using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Delegator;
using REAC.Application.Contracts.IAppServices;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Public)]
[PermissionRequired(Modules.Delegator)]

public class DelegatorController : ControllerBase
{
    private readonly IDelegatorService _delegatorService;
    public DelegatorController(IDelegatorService delegatorService)
    {
        _delegatorService = delegatorService;
    }

    [HttpGet(nameof(GetDelegators))]
    public async Task<ActionResult> GetDelegators([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.GetDelegators(pageIndex, pageSize, searchKey);
            return Ok(returnValue);
        }
        return BadRequest();

    }
    
    [HttpGet(nameof(GetLawyersDelegators))]
    public async Task<ActionResult> GetLawyersDelegators([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.GetLawyersDelegators(pageIndex, pageSize, searchKey);
            return Ok(returnValue);
        }
        return BadRequest();

    }
    [HttpGet(nameof(GetDelegatorById))]
    public async Task<ActionResult> GetDelegatorById(Guid delegatorId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.GetDelegatorById(delegatorId);
            return Ok(returnValue);
        }
        return BadRequest();

    }
    
    [HttpGet(nameof(GetLawyerDelegatorById))]
    public async Task<ActionResult> GetLawyerDelegatorById(Guid delegatorId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.GetDelegatorById(delegatorId);
            return Ok(returnValue);
        }
        return BadRequest();

    }

    [HttpPost(nameof(SaveDelegator))]
    public async Task<ActionResult> SaveDelegator(DelegatorDto delegatorDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.SaveDelegator(delegatorDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(SaveLawyerCitizenDelegator))]
    public async Task<ActionResult> SaveLawyerCitizenDelegator(LawyerDelegatorDto delegatorDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.AddLawyerCitizenAndDelagator(delegatorDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(SaveLawyerDelegator))]
    public async Task<ActionResult> SaveLawyerDelegator(LawyerDelegatorDto delegatorDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.AddLawyerDelagator(delegatorDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPost(nameof(ValidateDelegator))]
    public async Task<ActionResult> ValidateDelegator(ValidateDelegatorDto validateDelegatorDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.ValidateAgentDelegator(validateDelegatorDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    
    [HttpPost(nameof(ValidateLawyerDelegator))]
    public async Task<ActionResult> ValidateLawyerDelegator(ValidateLawyerDelegatorDto validateDelegatorDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.ValidateLawyerDelagator(validateDelegatorDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
        
    [HttpDelete(nameof(DeleteLawyerDelegator))]
    public async Task<ActionResult> DeleteLawyerDelegator(Guid delegatorId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _delegatorService.DeleteLawyerDelegator(delegatorId);
            return Ok(returnValue);
        }

        return BadRequest();
    }
}