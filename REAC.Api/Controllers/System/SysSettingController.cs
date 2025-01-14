using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;
using System.DirectoryServices.AccountManagement;

namespace REAC.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.Setting)]

public class SysSettingController : BaseController<SysSetting,
       SysSettingCreateUpdateDto, SysSettingCreateUpdateDto
       , Guid>
{
    private readonly ISysSettingService _settingService;
    public SysSettingController(ISysSettingService settingService, ISysSettingService sysSettingService) :
        base(sysSettingService)
    {
        _settingService = settingService;

    }

    [HttpGet("GetSettingByKey")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSettingByKey(string key)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _settingService.GetBySettingKey(key);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet("GetAllSetting")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSetting()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _settingService.GetByAllSetting();
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpGet("UpdateBySettingKey")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateBySettingKey(SysSetting updateSetting)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _settingService.UpdateBySettingKey(updateSetting);
            return Ok(returnValue);
        }

        return BadRequest();
    }
}