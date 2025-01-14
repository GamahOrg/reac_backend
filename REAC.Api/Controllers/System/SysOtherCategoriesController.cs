using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts;
using REAC.Application.Contracts.Dtos.System.GenderDtos;
using REAC.Application.Contracts.Dtos.System.SysOtherCategory;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Api.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.System)]
    [PermissionRequired(Modules.OtherCategory)]
    public class SysOtherCategoriesController : BaseController<OtherCategory,
       SysOtherCategoryCreateUpdateDto, SysOtherCategoryDto
       , Guid>
    {
        public SysOtherCategoriesController(ISysOtherCategoryService sysOtherCategoryService) :
    base(sysOtherCategoryService)
        {

        }
    }
}
