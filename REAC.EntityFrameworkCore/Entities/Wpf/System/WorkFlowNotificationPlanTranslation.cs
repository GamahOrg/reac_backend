using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowNotificationPlanTranslation
{
    [Comment("Description for Notification Plan")]
    public string NotificationPlan { get; set; } = null!;

    [Comment("Description for Language Code")]
    public string LanguageCode { get; set; } = null!;

}

