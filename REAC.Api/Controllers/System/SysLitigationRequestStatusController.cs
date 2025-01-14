using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.LitigationRequestStatusDtos;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.LitigationRequestStatus)]

public class SysLitigationRequestStatusController : BaseController<SysLitigationRequestStatus,
       SysLitigationRequestStatusCreateUpdateDto, SysLitigationRequestStatusDto
       , RequestStatus>
{

    public SysLitigationRequestStatusController(ISysLitigationRequestStatusService sysLitigationRequestStatusService) :
        base(sysLitigationRequestStatusService)
    {

    }
}