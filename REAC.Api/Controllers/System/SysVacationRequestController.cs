using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Security.Attributes;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Application.Contracts.Dtos.Citizen;
using Microsoft.AspNetCore.Authorization;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.System.VacationDtos;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
//[PermissionRequired(Modules.VacationRequest)]
[AllowAnonymous]

public class SysVacationRequestController : ControllerBase
{
    private readonly ISysVacationRequestService _sysVacationRequestService;
    public SysVacationRequestController(ISysVacationRequestService sysVacationRequestService)
    {
        _sysVacationRequestService = sysVacationRequestService;
       
    }

    [HttpPost(nameof(CreateVacationAsync))]
        public async Task<ActionResult> CreateVacationAsync(SysVacationRequestCreateUpdateDto holidayRequest)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _sysVacationRequestService.CreateVacationAsync(holidayRequest);
                return Ok(returnValue);
            }
            return BadRequest();
        }
    [HttpPost(nameof(ProcessVacationRequestAsync))]
    public async Task<ActionResult> ProcessVacationRequestAsync(Guid requestId, bool result)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysVacationRequestService.ProcessVacationRequestAsync(requestId, result);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(EmployeeList))]
    public async Task<ActionResult<IEnumerable<SysVacationRequestCreateUpdateDto>>> EmployeeList([FromQuery] int pageIndex, [FromQuery] int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysVacationRequestService.EmployeeList(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(AdminList))]
    public async Task<ActionResult<IEnumerable<SysVacationRequestCreateUpdateDto>>> AdminList([FromQuery] int pageIndex, [FromQuery] int pageSize)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysVacationRequestService.AdminList(pageIndex, pageSize);
            return Ok(returnValue);
        }

        return BadRequest();
    }


}