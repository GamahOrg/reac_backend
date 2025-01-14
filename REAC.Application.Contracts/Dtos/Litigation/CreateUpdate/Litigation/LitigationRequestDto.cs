using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation
{
    public class LitigationRequestDto
    {
        public string key { get; set; } = null!;
        public string SubKey { get; set; } = null!;
        public LitigationRequestContractType ContractType { get; set; }
        public LitigationRequestOwnerType LitigationRequestOwnerType { get; set; }
        public WafiRequestOwnerType? FirstPartyCapacity { get; set; }
        public WafiRequestOwnerType? SecondPartyCapacity { get; set; }
    }
}
