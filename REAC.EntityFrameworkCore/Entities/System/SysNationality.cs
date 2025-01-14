using Core.GenericRepository.Shared.BaseEntities;
using Core.Security.Entities;
using REAC.EntityFrameworkCore.Entities.Actor;
using System.ComponentModel.DataAnnotations;

namespace REAC.EntityFrameworkCore.Entities.System;

public class SysNationality : FullAuditedBaseEntity<Guid>
{
    public string NationalityName { get; set; } = null!;
    public int NationalityCode { get; set; }

    public ICollection<Citizen> Citizens { get; set; }
}
