using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Notification;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public partial class NotificationActorTypeSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysNotificationActorTypes.AnyAsync()) return;
            await context.SysNotificationActorTypes.AddRangeAsync(new List<SysNotificationActorType>
            {
                new SysNotificationActorType
                {
                    Id = NotificationActorType.Plaintiff,
                    ActorType = "مدعي",
                },
                 new SysNotificationActorType
                {
                    Id = NotificationActorType.Defendant,
                    ActorType = "مدعى عليه",
                },
                new SysNotificationActorType
                {
                    Id = NotificationActorType.Reformer,
                    ActorType = "مواطن",
                },
                new SysNotificationActorType
                {
                    Id = NotificationActorType.Arbitrator,
                    ActorType = "محكم",
                },
                new SysNotificationActorType
                {
                    Id = NotificationActorType.Employee,
                    ActorType = "موظف",
                },
                   new SysNotificationActorType
                {
                    Id = NotificationActorType.Applicant,
                    ActorType = "مقدم دعوي",
                },
                   new SysNotificationActorType
                {
                    Id = NotificationActorType.Agent,
                    ActorType = "وكيل",
                },
                   new SysNotificationActorType
                {
                    Id = NotificationActorType.Lawyer,
                    ActorType = "محامي",
                }
            });
            context.SaveChanges();
        }
    }
}