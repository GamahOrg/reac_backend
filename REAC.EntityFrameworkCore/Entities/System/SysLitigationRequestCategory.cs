using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestCategory : BaseEntity<LitigationRequestCategory>
{
    /// <summary>
    /// صلح – تحكيم-صلح وتحكيم
    /// </summary>
    public string CategoryName { get; set; } = null!;

    public string? AcknowledgmentContent { get; set; }

    public string? TermsAndConditions { get; set; }

    public Guid BpfId { get; set; }

    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<LitigationRequestNote> LitigationRequestNotes { get; set; } = new List<LitigationRequestNote>();
    public virtual ICollection<LitigationRequestCorrespondence> LitigationRequestCorrespondences { get; set; } = new List<LitigationRequestCorrespondence>();

    public virtual ICollection<SysLitigationRequestCategoryTranslation> Translations { get; set; } = new List<SysLitigationRequestCategoryTranslation>();

}
