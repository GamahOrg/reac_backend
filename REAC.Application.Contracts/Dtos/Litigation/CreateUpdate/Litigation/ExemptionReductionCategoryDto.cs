using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation
{
    public class ExemptionReductionCategoryDto
    {
        public Guid Id { get; set; }
        public Guid AttachmentId { get; set; }
        public SysFileManagerCreateUpdateDto? CategoryFile { get; set; }
        public bool IsExemptionReductionCategory { get; set; }
    }
}
