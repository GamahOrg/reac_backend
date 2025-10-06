using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.Wpf.Business;

public partial class WorkFlowLogTranslation
{
    [Comment("Description for Log Message")]
    public string LogMessage { get; set; } = null!;

    [Comment("Description for Language Code")]
    public string LanguageCode { get; set; } = null!;
}

