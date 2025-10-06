using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowNotificationDestinationTypeTranslation
{
    [Comment("Description for Destination Type")]
    public string DestinationType { get; set; } = null!;

    [Comment("Description for Language Code")]
    public string LanguageCode { get; set; } = null!;

}

