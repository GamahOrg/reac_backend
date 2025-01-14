using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.GenderDtos;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.DelegationRelationshipType)]

public class SysDelegationRelationshipTypeController : BaseController<SysDelegationRelationshipType,
       SysDelegationRelationshipTypeCreateUpdateDto, SysDelegationRelationshipTypeDto
       , DelegationRelationshipType>
{

    public SysDelegationRelationshipTypeController(ISysDelegationRelationshipTypeService sysDelegationRelationshipTypeService) :
        base(sysDelegationRelationshipTypeService)
    {

    }
}