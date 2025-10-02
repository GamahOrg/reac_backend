using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysLitigationRequestOwnerTypeTranslation
    {
        [Comment("Description for Litigation Request Owner Type")]
        public string LitigationRequestOwnerType { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

