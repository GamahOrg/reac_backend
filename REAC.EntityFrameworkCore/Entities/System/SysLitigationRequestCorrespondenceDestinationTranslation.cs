using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysLitigationRequestCorrespondenceDestinationTranslation
    {
        [Comment("Description for Destination")]
        public string Destination { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

