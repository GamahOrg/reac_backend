using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.Settings
{
    public class ExemptCategoriesDto : BaseEntity<Guid>
    {
        public string DiscountPercentage { get; set; }
        public bool ExemptionFromAdministrativeFees { get; set; }
        public bool ReducingConciliationAndArbitrationFees { get; set; }
    }
}
