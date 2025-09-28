using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysCitizenRequestStatus : BaseEntity<CitizenRequestStatus>
{
    [Comment("Description for Request Status")]
    public string RequestStatus { get; set; } = null!;

    public virtual ICollection<CitizenRequest> CitizenRequests { get; set; } = new List<CitizenRequest>();

    public virtual ICollection<SysCitizenRequestStatusTranslation> Translations { get; set; } = new List<SysCitizenRequestStatusTranslation>();

}

