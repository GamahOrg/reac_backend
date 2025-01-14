using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysLitigationRequestCorrespondenceStatus : BaseEntity<CorrespondenceStatus>
    {
        public string EnName { get; set; }
        public string ArName { get; set; }

        public virtual ICollection<LitigationRequestCorrespondence> LitigationRequestCorrespondences { get; set; } = new List<LitigationRequestCorrespondence>();

    }
}
