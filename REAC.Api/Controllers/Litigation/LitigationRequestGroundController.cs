using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Api.Controllers.Litigation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Litigation)]
[PermissionRequired(Modules.LitigationRequestGround)]

public class LitigationRequestGroundController : BaseController<LitigationRequestGround,
       LitigationRequestGroundCreateUpdateDto, LitigationRequestGroundCreateUpdateDto
       , Guid>
{

    public LitigationRequestGroundController(ILitigationRequestGroundService litigationRequestGroundService) :
        base(litigationRequestGroundService)
    {

    }
}