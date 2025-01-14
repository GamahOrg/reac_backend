using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Security.Attributes;
using REAC.Application.Contracts.IAppServices.Reconciliation;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Api.Controllers.Reconciliation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Reconciliation)]
[PermissionRequired(Modules.LitigationRequestExtendPeriodRequest)]

public class LitigationRequestExtendPeriodRequestController : BaseController<LitigationRequestExtendPeriodRequest,
       LitigationRequestExtendPeriodRequestCreateUpdateDto, LitigationRequestExtendPeriodRequestCreateUpdateDto
       , Guid>
{

    public LitigationRequestExtendPeriodRequestController(ILitigationRequestExtendPeriodRequestService litigationRequestExtendPeriodRequestService) :
        base(litigationRequestExtendPeriodRequestService)
    {

    }
}
