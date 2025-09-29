using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestContractType : BaseEntity<LitigationRequestContractType>
{
    [Comment("Name of the litigation request contract type")]
    public string TypeName { get; set; } = null!;
    [Comment("Duration of the reconciliation period in days for this contract type")]
    public int ReconciliationPeriod { get; set; }

    [Comment("Prefix symbol used for coding litigation requests of this contract type")]
    public string CodingPrefixSymbol { get; set; } = null!;

    [Comment("Content of the acknowledgment for this contract type")]
    public string? AcknowledgmentContent { get; set; }

    [Comment("Terms and conditions applicable to this contract type")]
    public string? TermsAndConditions { get; set; }

    public virtual ICollection<LitigationRequestContract> LitigationRequestContracts { get; set; } = new List<LitigationRequestContract>();

    public virtual ICollection<SysLitigationRequestContractTypeTranslation> Translations { get; set; } = new List<SysLitigationRequestContractTypeTranslation>();

}
