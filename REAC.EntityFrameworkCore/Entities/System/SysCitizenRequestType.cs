using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysCitizenRequestType : BaseEntity<CitizenRequestType>
{
    [Comment("Type of the name")]
    public string CitizenRequestTypeName { get; set; } = null!;

    [Comment("Unique identifier of the bpf")]
    public Guid BpfId { get; set; }

    public virtual ICollection<CitizenRequest> CitizenRequests { get; set; } = new List<CitizenRequest>();
    public virtual ICollection<SysCitizenRequestTypeTranslation> Translations { get; set; } = new List<SysCitizenRequestTypeTranslation>();

}


