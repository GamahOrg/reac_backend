using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowNotificationTypeTranslation
{
    [Comment("Description for Notification Type")]
    public string NotificationType { get; set; } = null!;

    [Comment("Description for Language Code")]
    public string LanguageCode { get; set; } = null!;

}

