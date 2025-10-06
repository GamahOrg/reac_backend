using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysLitigationRequestCorrespondenceStatus : BaseEntity<CorrespondenceStatus>
    {
        [Comment("Name of the en")]
        public string EnName { get; set; }
        [Comment("Name of the en")]
        public string ArName { get; set; }

        public virtual ICollection<LitigationRequestCorrespondence> LitigationRequestCorrespondences { get; set; } = new List<LitigationRequestCorrespondence>();

    }
}

