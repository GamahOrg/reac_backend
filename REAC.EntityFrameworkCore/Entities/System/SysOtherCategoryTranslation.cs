using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysOtherCategoryTranslation
    {
        [Comment("Category of the name")]
        public string CategoryName { get; set; } = null !;
        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

