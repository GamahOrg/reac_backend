using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysLitigationRequestContractTypeTranslation
    {
        [Comment("Type of the name")]
        public string TypeName { get; set; } = null!;

        [Comment("Content")]
        public string? AcknowledgmentContent { get; set; }

        [Comment("Description for Terms And Conditions")]
        public string? TermsAndConditions { get; set; }

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

