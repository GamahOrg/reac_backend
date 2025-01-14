using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public SysDepartmentController(ISysDepartmentService sysDepartmentService) :
        base(sysDepartmentService)
    {

    }
}