using REAC.EntityFrameworkCore.Entities.Arbitration;

namespace REAC.Application.Contracts.Dtos.Arbitration
{
    // Helper class for scoring arbitrators
    public class ArbitratorSelectionScore
    {
        public LitigationRequestArbitratorNominationList Arbitrator { get; set; }
        public int? TotalScore { get; set; }
        public int? ExperienceYears { get; set; }
        public int? RequestsCount { get; set; }
        public decimal? GeographicalPriority { get; set; } // 0=same area, 1=different city
    }

    public class ArbitratorChairmanSelectionScore
    {
        public LitigationRequestArbitratorsChairmanNominationList Arbitrator { get; set; }
        public int? TotalScore { get; set; }
        public int? ExperienceYears { get; set; }
        public int? RequestsCount { get; set; }
        public decimal? GeographicalPriority { get; set; } 
    }

    public class AddMemberRequestPayload
    {
        public Guid RequestId { get; set; }

        public Guid ArbitratorMemberId { get; set; }
    }
}

