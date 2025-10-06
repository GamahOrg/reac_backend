using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestCategory : BaseEntity<LitigationRequestCategory>
{
    /// <summary>
    /// صلح – تحكيم-صلح وتحكيم
    /// </summary>
    [Comment("Category of the name")]
    public string CategoryName { get; set; } = null!;

    [Comment("Content")]
    public string? AcknowledgmentContent { get; set; }

    [Comment("Description for Terms And Conditions")]
    public string? TermsAndConditions { get; set; }

    [Comment("Unique identifier of the bpf")]
    public Guid BpfId { get; set; }

    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<LitigationRequestNote> LitigationRequestNotes { get; set; } = new List<LitigationRequestNote>();
    public virtual ICollection<LitigationRequestCorrespondence> LitigationRequestCorrespondences { get; set; } = new List<LitigationRequestCorrespondence>();

    public virtual ICollection<SysLitigationRequestCategoryTranslation> Translations { get; set; } = new List<SysLitigationRequestCategoryTranslation>();

}

