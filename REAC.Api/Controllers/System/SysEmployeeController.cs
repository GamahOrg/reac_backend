using Core.Security.Attributes;
using Core.Security.Authorization;
using Core.Security.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.AppServices.System;
using REAC.Application.Contracts.Dtos.System.EmployeeDtos;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.Dtos.System.VacationDtos;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;
using Core.Shared.Responses;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.Employee)]
public class SysEmployeeController : BaseController<SysEmployee,
       SysEmployeeCreateUpdateDto, SysEmployeeDto
       , Guid>
{
    private readonly ISysEmployeeService _sysEmployeeService;


    public SysEmployeeController(ISysEmployeeService sysEmployeeService) :
        base(sysEmployeeService)
    {
        _sysEmployeeService = sysEmployeeService;
    }



    [HttpGet(nameof(List))]

    public async Task<ActionResult<IEnumerable<SysEmployeeListDto>>> List([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey, [FromQuery] bool? isActive)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysEmployeeService.List(pageIndex, pageSize, searchKey, isActive);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    //[AllowAnonymous]
    [HttpPost(nameof(CreateEmployee))]
    public async Task<ActionResult> CreateEmployee(SysEmployeeDto sysEmployeeDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysEmployeeService.CreateEmployee(sysEmployeeDto);
            return Ok(returnValue);
        }
        return BadRequest();
    }
    [HttpPut(nameof(UpdateEmployeeStatus))]
    public async Task<ActionResult> UpdateEmployeeStatus(string employeeId, bool isActive)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysEmployeeService.UpdateEmployeeStatus(Guid.Parse(employeeId), isActive);
            return Ok(returnValue);
        }

        return BadRequest();
    }
}
