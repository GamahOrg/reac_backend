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
[PermissionRequired(Modules.ArbitratorNominationListCount)]
[AllowAnonymous]

public class SysArbitratorNominationListCountController : BaseController<SysArbitratorNominationListCount,
       SysArbitratorNominationListCountCreateUpdateDto, SysArbitratorNominationListCountCreateUpdateDto
       , Guid>
{
    private readonly ISysArbitratorNominationListCountService _sysArbitratorNominationListCountService;
    public SysArbitratorNominationListCountController(ISysArbitratorNominationListCountService sysArbitratorNominationListCountService) :
         base(sysArbitratorNominationListCountService)
    {
        _sysArbitratorNominationListCountService = sysArbitratorNominationListCountService;
    }

}