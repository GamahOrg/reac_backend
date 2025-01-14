namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysLitigationRequestContractTypeTranslation
    {
        public string TypeName { get; set; } = null!;

        public string? AcknowledgmentContent { get; set; }

        public string? TermsAndConditions { get; set; }

        public string LanguageCode { get; set; } = null!;

    }
}