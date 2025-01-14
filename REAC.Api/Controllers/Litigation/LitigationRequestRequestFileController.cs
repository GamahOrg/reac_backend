using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Security.Attributes;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
[PermissionRequired(Modules.LitigationRequestRequestFile)]

public class LitigationRequestRequestFileController : BaseController<LitigationRequestRequestFile,
       LitigationRequestRequestFileCreateUpdateDto, LitigationRequestRequestFileCreateUpdateDto
       , Guid>
{

    public LitigationRequestRequestFileController(ILitigationRequestRequestFileService litigationRequestRequestFileService) :
        base(litigationRequestRequestFileService)
    {

    }
}