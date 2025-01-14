using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Litigation;

public partial class LitigationRequestSessionCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public DateTime SessionDatetime { get; set; }
    public string? SessionUrl { get; set; }

}
