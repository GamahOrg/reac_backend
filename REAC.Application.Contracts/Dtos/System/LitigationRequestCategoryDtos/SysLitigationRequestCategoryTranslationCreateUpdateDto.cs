namespace REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos
{
    public class SysLitigationRequestCategoryTranslationCreateUpdateDto
    {
        public string CategoryName { get; set; } = null!;

        public string? AcknowledgmentContent { get; set; }

        public string? TermsAndConditions { get; set; }

        public string LanguageCode { get; set; } = null!;

    }
}