using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysAreaTranslation
    {
        [Comment("Name of the area")]
        public string AreaName { get; set; } = null!;
        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;
    }
}

