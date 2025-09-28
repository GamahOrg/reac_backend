using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepsActorTypeTranslation
{
    [Comment("Description for Actor Type")]
    public string ActorType { get; set; } = null!;

    [Comment("Description for Language Code")]
    public string LanguageCode { get; set; } = null!;
}

