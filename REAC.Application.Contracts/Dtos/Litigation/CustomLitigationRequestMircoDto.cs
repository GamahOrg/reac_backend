
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class CustomLitigationRequestMircoDto : LitigationRequestMircoDto
    {
        public Guid RequestId { get; set; }

        public DateTime? ResponseOn { get; set; }
        public LitigationRequestReformerAssigningRequestStatus? Response { get; set; }
    }
    public class CustomLitigationRequestMircoAllDto : LitigationRequestMircoDto
    {
        public Guid RequestId { get; set; }
        public string? ReformerName { get; set; }
       public int? ReconciliationPeriod { get; set; } 
        public DateTime? ResponseOn { get; set; }
        public LitigationRequestReformerAssigningRequestStatus? Response { get; set; }
    }
    public class CustomLitigationRequestMircoArbitratorDto : LitigationRequestMircoDto
    {
        public Guid RequestId { get; set; }
        public ArbitratorAssigningRequestType? ArbitratorAssigningRequestType { get; set; }
        public DateTime? ResponseOn { get; set; }
        public LitigationRequestArbitratorAssigningRequestStatus? Response { get; set; }
    }
    public class CustomLitigationRequestMircoArbitratorAllDto : LitigationRequestMircoDto
    {
        public Guid RequestId { get; set; }
        public string? ArbitratorName { get; set; }
        public int? ArbitrationPeriod { get; set; }
        public ArbitratorAssigningRequestType? ArbitratorAssigningRequestType { get; set; }
        public DateTime? ResponseOn { get; set; }
        public LitigationRequestArbitratorAssigningRequestStatus? Response { get; set; }
    }
}
