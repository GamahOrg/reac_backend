using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysExemptionReductionCategoryTranslation
    {
        [Comment("Category of the name")]
        public string ExemptionReductionCategoryName { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

