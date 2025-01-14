namespace REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos
{
    public class SysLitigationRequestContractTypeTranslationCreateUpdateDto
    {
        public string TypeName { get; set; } = null!;

        public string? AcknowledgmentContent { get; set; }

        public string? TermsAndConditions { get; set; }

        public string LanguageCode { get; set; } = null!;

    }
}