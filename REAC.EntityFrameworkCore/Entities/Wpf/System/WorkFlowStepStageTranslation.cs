using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepStageTranslation
{
    [Comment("Name of the stage")]
    public string StageName { get; set; } = null!;

    [Comment("Language code for stage localization")]
    public string LanguageCode { get; set; } = null!;
}

