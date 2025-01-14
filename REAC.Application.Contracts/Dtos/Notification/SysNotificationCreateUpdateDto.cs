using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Notification
{
    public class SysNotificationCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid RequestId { get; set; }

        public byte RequestType { get; set; }

        public Guid EmployeeId { get; set; }

        public string Message { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public bool IsViewed { get; set; }

        public DateTime? ViewedOn { get; set; }
    }
}