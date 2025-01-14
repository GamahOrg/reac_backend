using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.Settings
{
    public class LitigationSettingsDto : BaseEntity<Guid>
    {
        public int Id { get; set; }
        public string LitigationGracePeriod { get; set; }
        public List<ExemptCategoriesDto> ExemptCategories { get; set; }
    }
}

