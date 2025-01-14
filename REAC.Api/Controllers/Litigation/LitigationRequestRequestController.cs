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
[PermissionRequired(Modules.LitigationRequestRequest)]

public class LitigationRequestRequestController : BaseController<LitigationRequestRequest,
       LitigationRequestRequestCreateUpdateDto, LitigationRequestRequestCreateUpdateDto
       , Guid>
{

    public LitigationRequestRequestController(ILitigationRequestRequestService litigationRequestRequestService) :
        base(litigationRequestRequestService)
    {

    }
}
