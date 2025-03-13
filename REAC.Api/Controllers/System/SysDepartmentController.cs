using Core.Security.Attributes;
using Core.Security.Authorization;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.AppServices.System;
using REAC.Application.Contracts.Dtos.System.DepartmentDtos;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.Department)]
public class SysDepartmentController : BaseController<SysDepartment,
       SysDepartmentCreateUpdateDto, SysDepartmentDto
       , Guid>
{
    private readonly ISysDepartmentService _sysDepartmentService;


    public SysDepartmentController(ISysDepartmentService sysDepartmentService) :
        base(sysDepartmentService)
    {
        _sysDepartmentService = sysDepartmentService;
    }

    [HttpGet(nameof(ToggleAutoDistributionOnAndOff))]
    public async Task<ActionResult> ToggleAutoDistributionOnAndOff(int departmentType, int status)
    {

        var returnValue = await _sysDepartmentService.ToggleAutoDistributionOnAndOff(departmentType, status);
        return Ok(returnValue);

    }
    
    [HttpGet(nameof(GetDistributionModeByDepartmentType))]
    public async Task<ActionResult> GetDistributionModeByDepartmentType(int departmentType)
    {

        var returnValue = await _sysDepartmentService.GetDistributionModeByDepartmentType(departmentType);
        return Ok(returnValue);

    }
}