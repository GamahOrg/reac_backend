using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.Bank)]
[AllowAnonymous]

public class SysBankController : BaseController<SysBank,
       SysBankCreateUpdateDto, SysBankCreateUpdateDto
       , Guid>
{
    private readonly ISysBankService _sysBankService;
    public SysBankController(ISysBankService sysBankService) :
         base(sysBankService)
    {
        _sysBankService = sysBankService;
    }

}