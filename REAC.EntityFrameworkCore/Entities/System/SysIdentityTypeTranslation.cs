using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysIdentityTypeTranslation
    {
        [Comment("Description for Identity Type")]
        public string IdentityType { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

