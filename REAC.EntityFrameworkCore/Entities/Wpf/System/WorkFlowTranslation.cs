using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowTranslation
{
    [Comment("Name of the work flow")]
    public string WorkFlowName { get; set; } = null!;

    [Comment("Language code for workflow localization")]
    public string LanguageCode { get; set; } = null!;
}

