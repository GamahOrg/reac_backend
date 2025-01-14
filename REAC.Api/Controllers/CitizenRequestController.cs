using AutoMapper;
using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.CitizenRequests;
using REAC.Application.Contracts.IAppServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Public)]
[PermissionRequired(Modules.Delegator)]

public class CitizenRequestController : ControllerBase
{

    protected IMapper _mapper;
    private readonly ICitizenRequestService _citizenRequestService;
    public CitizenRequestController(ICitizenRequestService citizenRequestService, IMapper mapper)
    {
        _mapper = mapper;
        _citizenRequestService = citizenRequestService;
    }

    [HttpGet(nameof(GetCitizenRequests))]
    public async Task<ActionResult> GetCitizenRequests(int pageIndex, int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenRequestService.GetCitizenRequests(pageIndex, pageSize, false);
            return Ok(returnValue);
        }
        return BadRequest();

    }

    [HttpGet(nameof(GetCitizenRequestsForEmployee))]
    public async Task<ActionResult> GetCitizenRequestsForEmployee(int pageIndex, int pageSize, CitizenRequestType citizenRequestType)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenRequestService.GetCitizenRequests(pageIndex, pageSize, true, citizenRequestType);
            return Ok(returnValue);
        }
        return BadRequest();

    }
    [HttpGet(nameof(GetCitizenRequestById))]
    public async Task<ActionResult> GetCitizenRequestById(Guid requestById)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenRequestService.GetCitizenRequestById(requestById);
            return Ok(returnValue);
        }
        return BadRequest();

    }
    [HttpPost(nameof(SaveCitizenRequest))]
    public async Task<ActionResult> SaveCitizenRequest(CitizenRequestCreateUpdateDto citizenRequestCreateUpdateDto)
    {
        if (ModelState.IsValid)
        {
            //var request = _mapper.Map<CitizenRequest>(citizenRequestCreateUpdateDto);
            var returnValue = await _citizenRequestService.SaveCitizenRequest(citizenRequestCreateUpdateDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpPost(nameof(ExecuteAction))]
    public async Task<ActionResult> ExecuteAction(CitizenRequestActionModel model)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _citizenRequestService.ExecuteAction(model.CitizenRequestAction, model.RequestId, model.Notes);
            return Ok(returnValue);
        }

        return BadRequest();
    }
}