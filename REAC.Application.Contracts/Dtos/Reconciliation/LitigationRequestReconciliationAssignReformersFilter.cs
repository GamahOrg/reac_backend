using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Reconciliation
{
    public class LitigationRequestReconciliationAssignReformersFilter
    {
        public string? ReformerName { get; set; }
        public Guid? AreaId { get; set; }
        public Guid? CityId { get; set; }
    }
}
