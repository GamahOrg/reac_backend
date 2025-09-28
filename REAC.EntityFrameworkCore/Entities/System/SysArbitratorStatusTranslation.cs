using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorStatusTranslation
    {
        [Comment("Status of the name")]
        public string StatusName { get; set; } = null!;
        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

