using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.CityDto;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.LitigationRequest)]
[AllowAnonymous]

public class SysCityController : BaseController<SysCity,
       SysCityDto, SysCityDto
       , Guid>
{
    private readonly ISysCityService _litigationCityService;
    public SysCityController(ISysCityService litigationRequestService, ISysCityService litigationService) :
         base(litigationRequestService)
    {
        _litigationCityService = litigationService;
    }
}