using Core.GenericRepository.Shared.BaseEntities;
using Core.Security.Entities;
using REAC.EntityFrameworkCore.Entities.Actor;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public class SysNationality : FullAuditedBaseEntity<Guid>
{
    [Comment("Name of the nationality")]
    public string NationalityName { get; set; } = null!;
    [Comment("Description for Nationality Code")]
    public int NationalityCode { get; set; }

    public ICollection<Citizen> Citizens { get; set; }
}

