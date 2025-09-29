using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepTranslation
{
    [Comment("Name of the step")]
    public string StepName { get; set; } = null!;
    [Comment("Log message displayed when the workflow step is created")]
    public string CreationLogMessage { get; set; } = null!;

    [Comment("Language code for workflow step localization")]
    public string LanguageCode { get; set; } = null!;
}

