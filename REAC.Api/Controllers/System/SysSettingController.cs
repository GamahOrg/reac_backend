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
    
    [HttpGet("GetReconciliationFeesSettings")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReconciliationFeesSettings()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _settingService.GetReconciliationFeesSettings();
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPut("UpdateBySettingKey")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateBySettingKey([FromBody]SysSetting updateSetting)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _settingService.UpdateBySettingKey(updateSetting);
            return Ok(returnValue);
        }

        return BadRequest();
    }

    [HttpPut("UpdateSettingsAsync")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateSettingsAsync(Dictionary<string, string> settingsToUpdate)
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _settingService.UpdateSettingsAsync(settingsToUpdate);
            return Ok(returnValue);
        }

        return BadRequest();
    }
    [HttpGet("GetReconciliationSetting")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReconciliationSetting()
    {
        if (ModelState.IsValid)
        {
            var returnValue = await _settingService.GetReconciliationSetting();
            return Ok(returnValue);
        }

        return BadRequest();
    }
}