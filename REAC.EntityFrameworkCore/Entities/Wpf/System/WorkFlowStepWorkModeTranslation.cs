using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepWorkModeTranslation
{
    [Comment("Work mode description for workflow steps")]
    public string WorkMode { get; set; } = null!;

    [Comment("Language code for work mode localization")]
    public string LanguageCode { get; set; } = null!;
}

