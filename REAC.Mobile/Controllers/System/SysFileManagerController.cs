using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Mobile.Controllers.System;

[Route("api/mobile/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.System)]
[PermissionRequired(Modules.FileManager)]

public class SysFileManagerController : BaseController<SysFileManager,
       SysFileManagerCreateUpdateDto, SysFileManagerCreateUpdateDto
       , Guid>
{
    private readonly ISysFileManagerService _fileService;
    public SysFileManagerController(ISysFileManagerService sysFileManagerService, ISysFileManagerService fileService) :
        base(sysFileManagerService)
    {
        _fileService = fileService;

    }
    [HttpPost]
    [Route(nameof(GetByIds))]
    public async Task<IActionResult> GetByIds([FromBody] FileParameterDto input)
    {
        var files = await _fileService.GetAsync(input.List);
        // var mapped = Mapper.Map<List<FileDto>>(files);
        return Ok(files);
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [AllowAnonymous]
    [Route(nameof(Upload))]
    public async Task<IActionResult> Upload([FromForm] UploadFilesDto input)
    {
        var result = await _fileService.UploadAsync(input.Files);
        return Ok(result);
    }

    [HttpDelete]
    [Route(nameof(Remove))]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _fileService.RemoveAsync(id);
        return Ok("");
    }
} 

