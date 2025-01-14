using System;
using System.Collections.Generic;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepEventTranslation
{
    public string? EventName { get; set; }

    public string? LogMessage { get; set; }

    public string LanguageCode { get; set; } = null!;

}
