using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Public)]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IJwtReader _jwtReader;

        public NotificationController(INotificationService notificationService, IJwtReader jwtReader)
        {
            _notificationService = notificationService;
            _jwtReader = jwtReader;
        }

        [HttpGet]
        [Route(nameof(GetNotificationListForPortal))]
        public async Task<IActionResult> GetNotificationListForPortal()
        {

            return Ok(await _notificationService.GetNotificationListForPortal());
        }

        [HttpGet]
        [Route(nameof(GetNotificationListForBackOffice))]
        public async Task<IActionResult> GetNotificationListForBackOffice()
        {
            return Ok(await _notificationService.GetNotificationListForBackOffice());
        }

        [HttpGet]
        [Route(nameof(MarkNotificationViewed) + "/{notificationId}")]
        public async Task<IActionResult> MarkNotificationViewed(Guid notificationId)
        {
            return Ok(await _notificationService.MarkNotificationViewed(notificationId));
        }

        [HttpGet]
        [Route(nameof(GetNotificationListForPortalPaged))]
        public async Task<IActionResult> GetNotificationListForPortalPaged(int pageIndex, int pageSize)
        {
            return Ok(await _notificationService.GetNotificationListForPortal(pageIndex, pageSize));
        }

        [HttpGet]
        [Route(nameof(GetNotificationListForBackOfficePaged))]
        public async Task<IActionResult> GetNotificationListForBackOfficePaged(int pageIndex, int pageSize)
        {
            return Ok(await _notificationService.GetNotificationListForBackOffice(pageIndex, pageSize));
        }
    }
}
