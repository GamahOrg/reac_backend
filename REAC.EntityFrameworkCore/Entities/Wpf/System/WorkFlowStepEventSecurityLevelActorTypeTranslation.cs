using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.Wpf.System
{
    public class WorkFlowStepEventSecurityLevelActorTypeTranslation
    {
        [Comment("Description for Security Level Actor Type")]
        public string SecurityLevelActorType { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;
    }
}

