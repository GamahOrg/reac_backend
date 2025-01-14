using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.LitigationRequestOwnerTypeDtos;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.LitigationRequestOwnerType)]

public class SysLitigationRequestOwnerTypeController : BaseController<SysLitigationRequestOwnerType,
       SysLitigationRequestOwnerTypeCreateUpdateDto, SysLitigationRequestOwnerTypeDto
       , LitigationRequestOwnerType>
{

    public SysLitigationRequestOwnerTypeController(ISysLitigationRequestOwnerTypeService sysLitigationRequestOwnerTypeService) :
        base(sysLitigationRequestOwnerTypeService)
    {

    }
}