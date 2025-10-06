using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.Wpf.System
{
    public class WorkFlowStepEventSendNotesModeTranslation
    {
        [Comment("Notes")]
        public string SendNotesMode { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;
    }
}

