using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.Notification;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysNotificationPlanService : IBasicService<SysNotificationPlan, Guid>
    {
        Task<BaseResponse> ExecuteNotificationPlan(Guid notificationPlanId, Guid requestId);
    }
}
