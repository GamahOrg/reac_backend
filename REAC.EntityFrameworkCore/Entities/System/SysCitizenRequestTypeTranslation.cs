using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysCitizenRequestTypeTranslation
    {
        [Comment("Type of the name")]
        public string CitizenRequestTypeName { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;
    }
}

