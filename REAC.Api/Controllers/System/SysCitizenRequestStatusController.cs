using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.CitizenRequestStatusDtos;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.CitizenRequestStatus)]

public class SysCitizenRequestStatusController : BaseController<SysCitizenRequestStatus,
       SysCitizenRequestStatusCreateUpdateDto, SysCitizenRequestStatusDto
       , CitizenRequestStatus>
{

    public SysCitizenRequestStatusController(ISysCitizenRequestStatusService sysCitizenRequestStatusService) :
        base(sysCitizenRequestStatusService)
    {

    }
}