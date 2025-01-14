using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.RegistrationRequest)]

public class SysRegistrationRequestController : BaseController<SysRegistrationRequest,
       SysRegistrationRequestCreateUpdateDto, SysRegistrationRequestCreateUpdateDto
       , Guid>
{

    public SysRegistrationRequestController(ISysRegistrationRequestService sysRegistrationRequestService) :
        base(sysRegistrationRequestService)
    {

    }
}