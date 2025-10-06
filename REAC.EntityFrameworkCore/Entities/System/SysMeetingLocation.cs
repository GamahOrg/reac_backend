using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysMeetingLocation : BaseEntity<Guid>
    {
        [Comment("Name of the meeting location")]
        public string MeetingLocationName { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitrationReport> LitigationRequestArbitrationReports { get; set; } = new List<LitigationRequestArbitrationReport>();

    }
}

