using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.MeetingLocation)]
[AllowAnonymous]

public class SysMeetingLocationController : BaseController<SysMeetingLocation,
       SysMeetingLocationCreateUpdateDto, SysMeetingLocationCreateUpdateDto
       , Guid>
{
    private readonly ISysMeetingLocationService _sysBankService;
    public SysMeetingLocationController(ISysMeetingLocationService sysBankService) :
         base(sysBankService)
    {
        _sysBankService = sysBankService;
    }

}