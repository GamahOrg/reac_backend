using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysLitigationRequestStatusTranslation
    {
        [Comment("Description for Request Status")]
        public string RequestStatus { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

