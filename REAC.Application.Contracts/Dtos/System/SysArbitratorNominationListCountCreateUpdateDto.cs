using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System
{
    public class SysArbitratorNominationListCountCreateUpdateDto : BaseEntity<Guid>
    {
        public int ArbitratorCount { get; set; }
        public int ArbitratorNominationListCount { get; set; }
        public int ArbitratorChairmanNominationListCount { get; set; }
    }

}
