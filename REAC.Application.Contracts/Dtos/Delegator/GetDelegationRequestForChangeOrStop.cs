

using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Delegator
{
    public class GetDelegationRequestForChangeOrStop
    {
        public Guid Id { get; set; }
        public Guid LitigationRequestId { get; set; }
        public string LitigationRequestCode { get; set; }
        public string DelegatorName { get; set; }
        public LitigationRequestParty DelegationParty { get; set; }
    }
}
