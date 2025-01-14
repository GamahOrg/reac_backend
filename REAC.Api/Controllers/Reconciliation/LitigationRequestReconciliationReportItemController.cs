using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices.Reconciliation;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Api.Controllers.Reconciliation;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = APISections.Reconciliation)]
[PermissionRequired(Modules.LitigationRequestMeetingReconciliationItem)]

public class LitigationRequestReconciliationReportItemController : BaseController<LitigationRequestReconciliationReportItem,
       LitigationRequestReconciliationReportItemCreateUpdateDto, LitigationRequestReconciliationReportItemCreateUpdateDto
       , Guid>
{
    public LitigationRequestReconciliationReportItemController(ILitigationRequestReconciliationReportItemService litigationRequestMeetingReconciliationItemService) :
       base(litigationRequestMeetingReconciliationItemService)
    {

    }
}