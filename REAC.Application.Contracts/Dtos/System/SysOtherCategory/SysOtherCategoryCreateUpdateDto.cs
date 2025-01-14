using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System.SysOtherCategory
{
    public class SysOtherCategoryCreateUpdateDto: SysOtherCategoryDto
    {
        public List<SysOtherCategoryCreateUpdateDtoTranslations> Translations { get; set; } = new();
    }
}
