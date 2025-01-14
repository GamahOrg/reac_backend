using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.CitizenRequests.CreateUpdate
{

    public partial class CitizenRequestReconciliationCenterDegreeFileCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid ReconciliationCenterDegreeFileId { get; set; }

    }
}
