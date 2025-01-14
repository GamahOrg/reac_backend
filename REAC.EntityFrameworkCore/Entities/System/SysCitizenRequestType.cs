using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysCitizenRequestType : BaseEntity<CitizenRequestType>
{
    public string CitizenRequestTypeName { get; set; } = null!;

    public Guid BpfId { get; set; }

    public virtual ICollection<CitizenRequest> CitizenRequests { get; set; } = new List<CitizenRequest>();
    public virtual ICollection<SysCitizenRequestTypeTranslation> Translations { get; set; } = new List<SysCitizenRequestTypeTranslation>();

}

