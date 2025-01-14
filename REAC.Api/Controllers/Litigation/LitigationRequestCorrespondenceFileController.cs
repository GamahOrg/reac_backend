using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
[PermissionRequired(Modules.LitigationRequestCorrespondenceFile)]

public class LitigationRequestCorrespondenceFileController : BaseController<LitigationRequestCorrespondenceFile,
       LitigationRequestCorrespondenceFileCreateUpdateDto, LitigationRequestCorrespondenceFileCreateUpdateDto
       , Guid>
{

    public LitigationRequestCorrespondenceFileController(ILitigationRequestCorrespondenceFileService litigationRequestCorrespondenceFileService) :
        base(litigationRequestCorrespondenceFileService)
    {

    }
}
