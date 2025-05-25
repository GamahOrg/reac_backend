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
using REAC.Application.Contracts.Dtos.System.DepartmentDtos;
using REAC.Domain.Shared.Enum;

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
    
    [HttpGet(nameof(GetEmployeeByDepartmentType))]

    public async Task<ActionResult<IEnumerable<SysEmployeeListDto>>> GetEmployeeByDepartmentType(DepartmentType departmentType)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysEmployeeService.GetAllEmployeeByDepartmentType(departmentType);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet(nameof(GetDistributioonMode))]

    public async Task<ActionResult<IEnumerable<SysDepartmentDistributionModeDto>>>GetDistributioonMode()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysEmployeeService.GetDistributioonMode();
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [AllowAnonymous]
    [HttpGet(nameof(GetByIdAsync))]

    public async Task<ActionResult<IEnumerable<SysEmployeeGetUpdateDto>>> GetByIdAsync(Guid employeeId)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysEmployeeService.GetByIdAsync(employeeId);
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

    [HttpPut(nameof(UpdateDistributioonMode))]
    public async Task<ActionResult> UpdateDistributioonMode(DepartmentDistributionMode distributionMode)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysEmployeeService.UpdateDistributioonMode(distributionMode);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [AllowAnonymous]
    [HttpPut(nameof(UpdateEmployee))]
    public async Task<ActionResult> UpdateEmployee(SysEmployeeGetUpdateDto sysEmployeeDto)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _sysEmployeeService.UpdateEmployee(sysEmployeeDto);
            return Ok(returnValue);
        }

        return BadRequest();
    }
}
