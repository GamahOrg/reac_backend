using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.AreaDto;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.Area)]
[AllowAnonymous]

public class SysAreaController : BaseController<SysArea,
       SysAreaDto, SysAreaDto
       , Guid>
{
    private readonly ISysAreaService _areaService;
    public SysAreaController(ISysAreaService areaService) :
         base(areaService)
    {
        _areaService = areaService;
    }

}