using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.Wpf.System
{
    public class WorkFlowStepEventSecurityLevelTranslation
    {
        [Comment("Description for Security Level")]
        public string SecurityLevel { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;
    }
}

