using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestContractType : BaseEntity<LitigationRequestContractType>
{
    public string TypeName { get; set; } = null!;
    public int ReconciliationPeriod { get; set; }

    public string CodingPrefixSymbol { get; set; } = null!;

    public string? AcknowledgmentContent { get; set; }

    public string? TermsAndConditions { get; set; }

    public virtual ICollection<LitigationRequestContract> LitigationRequestContracts { get; set; } = new List<LitigationRequestContract>();

    public virtual ICollection<SysLitigationRequestContractTypeTranslation> Translations { get; set; } = new List<SysLitigationRequestContractTypeTranslation>();

}
