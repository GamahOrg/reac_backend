using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;

namespace REAC.Application.Contracts.IAppServices.Notification
{
    public interface INotificationService
    {
        Task<BaseResponse> GetNotificationListForPortal();
        Task<BaseResponse> GetNotificationListForBackOffice();
        Task<BaseResponse> GetNotificationListForPortal(int pageIndex, int pageSize);
        Task<BaseResponse> GetNotificationListForBackOffice(int pageIndex, int pageSize);
        Task<BaseResponse> MarkNotificationViewed(Guid notificationId);
        Task<BaseResponse> SendNotifications(SendEmailDto email, SMSRequestDto sms, List<SysNotificationDto> notifications);
    }
}
