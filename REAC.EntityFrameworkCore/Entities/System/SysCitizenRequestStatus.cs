using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysCitizenRequestStatus : BaseEntity<CitizenRequestStatus>
{
    public string RequestStatus { get; set; } = null!;

    public virtual ICollection<CitizenRequest> CitizenRequests { get; set; } = new List<CitizenRequest>();

    public virtual ICollection<SysCitizenRequestStatusTranslation> Translations { get; set; } = new List<SysCitizenRequestStatusTranslation>();

}
