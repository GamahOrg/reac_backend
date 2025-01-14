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
[PermissionRequired(Modules.LitigationRequestReconciliationMeeting)]

public class LitigationRequestReconciliationReportController : BaseController<LitigationRequestReconciliationReport,
       LitigationRequestReconciliationReportCreateUpdateDto, LitigationRequestReconciliationReportCreateUpdateDto
       , Guid>
{

    public LitigationRequestReconciliationReportController(ILitigationRequestReconciliationReportService litigationRequestReconciliationMeetingService) :
        base(litigationRequestReconciliationMeetingService)
    {

    }
}