using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationCompanyRepresentative : BaseEntity<Guid>
    {
        public virtual ICollection<Citizen> Citizens { get; set; } = new List<Citizen>();
    }
}
