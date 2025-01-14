using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestMircoDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public int RequestTypeId { get; set; }
    public int? ArbitrationPeriod { get; set; }
    public int? ReconciliationPeriod { get; set; }

    public string? Status { get; set; } = null!;
    public string IsDone { get; set; } = null!;
    public ApproveRejectStatus Response { get; set; }


}
public partial class LitigationRequestMircoArbitratorDto : BaseEntity<Guid>
{
    public Guid LitigationRequestArbitratorListId { get; set; }
    public Guid? ArbitratoId { get; set; }

    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public ArbitratorAssigningRequestType ArbitratorAssigningRequestTypeId { get; set; }
    public string ArbitratorName { get; set; } = null!;

}
public partial class LitigationRequestMircoExplanationRequestDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public string? Status { get; set; } = null!;
    public string IsDone { get; set; } = null!;
    public string IsResponseDone { get; set; } = null!;
    public ApproveRejectStatus Response { get; set; }

    public LitigationRequestParty RequestParty { get; set; }



}

public partial class LitigationRequestMircoForArbitratorsCountDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public string? Status { get; set; } = null!;
    public int? ArbitratorsCount { get; set; }
    public string IsDone { get; set; } = null!;
}
public partial class LitigationRequestMircoForAllArbitratorsCountDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public string? Status { get; set; } = null!;
    public int? FirstPartyArbitratorsCount { get; set; }
    public int? SecondPartyArbitratorsCount { get; set; }
    public int? ArbitratorsCount { get; set; }
    public string IsDone { get; set; } = null!;
}
public partial class LitigationRequestMircoForArbitratorsSelectionDurationDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public string? Status { get; set; } = null!;
    public int? ArbitratorsSelectionDuration { get; set; }
    public string IsDone { get; set; } = null!;
}
public partial class LitigationRequestMircoForRequestSessionDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public DateTime SessionDatetime { get; set; }
    public string? Url { get; set; }
    public long? MeetingId { get; set; }
}

public partial class LitigationRequestMircoForRequestReformerDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public Guid? ReformerId { get; set; }

    public string? ReformerName { get; set; }
}

public partial class LitigationRequestMircoForRequestArbitratorDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public DateTime RequestDate { get; set; }
    public string FirstPartyName { get; set; } = null!;
    public string SecondPartyName { get; set; } = null!;
    public string RequestTypeName { get; set; } = null!;
    public Guid? ArbitratorId { get; set; }

    public string? ArbitratorName { get; set; }
}
