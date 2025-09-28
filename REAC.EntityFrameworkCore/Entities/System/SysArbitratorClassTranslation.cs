using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysArbitratorClassTranslation
    {
        [Comment("Name of the arbitrator class")]
        public string ArbitratorClassName { get; set; } = null!;
        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;
    }
}

