using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Arbitration;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysMeetingLocation : BaseEntity<Guid>
    {
        public string MeetingLocationName { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitrationReport> LitigationRequestArbitrationReports { get; set; } = new List<LitigationRequestArbitrationReport>();

    }
}
