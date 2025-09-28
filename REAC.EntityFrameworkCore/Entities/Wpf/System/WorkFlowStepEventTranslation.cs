using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepEventTranslation
{
    [Comment("Name of the event")]
    public string? EventName { get; set; }

    [Comment("Description for Log Message")]
    public string? LogMessage { get; set; }

    [Comment("Description for Language Code")]
    public string LanguageCode { get; set; } = null!;

}

