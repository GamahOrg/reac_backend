using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public class DefaultWorkFlowDataCreator
    {
        public static REACDBContext? _context;
        public static async Task CreateAsync(REACDBContext context)
        {
            _context = context;
            if (!context.WorkFlowStepEventSecurityLevel.Any())
            {
                await context.WorkFlowStepEventSecurityLevel.AddRangeAsync(GetWorkFlowStepEventSecurityLevel());
                await context.SaveChangesAsync();
            }

            if (!context.WorkFlowStepEventSecurityLevelActorTypes.Any())
            {
                await context.WorkFlowStepEventSecurityLevelActorTypes.AddRangeAsync(GetWorkFlowStepEventSecurityLevelActorTypes());
                await context.SaveChangesAsync();
            }

            if (!context.WorkFlowStepEventActionModes.Any())
            {
                await context.WorkFlowStepEventActionModes.AddRangeAsync(GetWorkFlowStepEventActionModes());
                await context.SaveChangesAsync();
            }

            if (!context.WorkFlowStepWorkModes.Any())
            {
                await context.WorkFlowStepWorkModes.AddRangeAsync(GetWorkFlowStepWorkModes());
                await context.SaveChangesAsync();
            }

            if (!context.WorkFlowStepStages.Any())
            {
                await context.WorkFlowStepStages.AddRangeAsync(GetWorkFlowStepStages());
                await context.SaveChangesAsync();
            }

            if (!context.WorkFlowStepsActorTypes.Any())
            {
                await context.WorkFlowStepsActorTypes.AddRangeAsync(GetWorkFlowStepsActorTypes());
                await context.SaveChangesAsync();
            }

            if (!context.WorkFlowNotificationPlans.Any())
            {
                await context.WorkFlowNotificationPlans.AddRangeAsync(GetWorkFlowNotificationPlans());
                await context.SaveChangesAsync();
            }

            if (!context.WorkFlows.Any())
            {
                await context.WorkFlows.AddRangeAsync(GetWorkFlows());
                await context.SaveChangesAsync();
            }

        }

        #region WorkFlow Step Event Security Level
        private static List<WorkFlowStepEventSecurityLevel> GetWorkFlowStepEventSecurityLevel()
        {
            return new List<WorkFlowStepEventSecurityLevel> {
                new WorkFlowStepEventSecurityLevel()
                {
                    Id = EventSecurityLevel.Specified,
                    SecurityLevel = "محدد",
                    Translations = new List<WorkFlowStepEventSecurityLevelTranslation>()
                    {
                        new WorkFlowStepEventSecurityLevelTranslation()
                        {
                            LanguageCode = "en-US",
                            SecurityLevel = "Specified"
                        },
                        new WorkFlowStepEventSecurityLevelTranslation()
                        {
                            LanguageCode = "ar-SA",
                            SecurityLevel = "محدد"
                        }
                    }
                },
                new WorkFlowStepEventSecurityLevel()
                {
                    Id = EventSecurityLevel.UnSpecified,
                    SecurityLevel = "غير محدد",
                    Translations = new List<WorkFlowStepEventSecurityLevelTranslation>()
                    {
                        new WorkFlowStepEventSecurityLevelTranslation()
                        {
                            LanguageCode = "en-US",
                            SecurityLevel = "UnSpecified"
                        },
                        new WorkFlowStepEventSecurityLevelTranslation()
                        {
                            LanguageCode = "ar-SA",
                            SecurityLevel = "غير محدد"
                        }
                    }
                }
            };
        }
        #endregion

        #region Work Flow Step Work Mode
        private static List<WorkFlowStepWorkMode> GetWorkFlowStepWorkModes()
        {
            return new List<WorkFlowStepWorkMode> {
                new WorkFlowStepWorkMode()
                {
                    Id = StepWorkMode.None,
                    WorkMode = "لا شئ",
                    Translations = new List<WorkFlowStepWorkModeTranslation>()
                    {
                        new WorkFlowStepWorkModeTranslation()
                        {
                            LanguageCode = "en-US",
                            WorkMode = "None"
                        },
                        new WorkFlowStepWorkModeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            WorkMode = "لا شئ"
                        }
                    }
                }
            };
        }
        #endregion

        #region WorkFlow Event Action Mode
        private static List<WorkFlowStepEventActionMode> GetWorkFlowStepEventActionModes()
        {
            return new List<WorkFlowStepEventActionMode> {
                new WorkFlowStepEventActionMode()
                {
                    Id = EventActionMode.GoNextStep,
                    ActionMode = "الخطوة التالية",
                    Translations = new List<WorkFlowStepEventActionModeTranslation>()
                    {
                        new WorkFlowStepEventActionModeTranslation()
                        {
                            LanguageCode = "en-US",
                            ActionMode = "Go Next Step",
                        },
                        new WorkFlowStepEventActionModeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            ActionMode = "الخطوة التالية",
                        }
                    }
                },
                new WorkFlowStepEventActionMode()
                {
                    Id = EventActionMode.GoPreviousStep,
                    ActionMode = "الخطوة السابقة",
                    Translations = new List<WorkFlowStepEventActionModeTranslation>()
                    {
                        new WorkFlowStepEventActionModeTranslation()
                        {
                            LanguageCode = "en-US",
                            ActionMode = "Previos Step",
                        },
                        new WorkFlowStepEventActionModeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            ActionMode = "الخطوة السابقة",
                        }
                    }
                },
                new WorkFlowStepEventActionMode()
                {
                    Id = EventActionMode.SpecificStep,
                    ActionMode = "خطوة محددة",
                    Translations = new List<WorkFlowStepEventActionModeTranslation>()
                    {
                        new WorkFlowStepEventActionModeTranslation()
                        {
                            LanguageCode = "en-US",
                            ActionMode = "Specific Step",
                        },
                        new WorkFlowStepEventActionModeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            ActionMode = "خطوة محددة",
                        }
                    }
                }
            };
        }
        #endregion

        #region WorkFlow Step Stage
        private static List<WorkFlowStepStage> GetWorkFlowStepStages()
        {
            return new List<WorkFlowStepStage> {
                new WorkFlowStepStage()
                {
                    Id = Guid.Parse("124e0e29-31e4-47f1-9536-aa35914b57e3"),
                    StageName = "مرحلة التدقيق",
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    IsDeleted = false,
                    Translations = new List<WorkFlowStepStageTranslation>()
                    {
                        new WorkFlowStepStageTranslation()
                        {
                            LanguageCode = "en-US",
                            StageName = "Studing Stage"
                        },
                        new WorkFlowStepStageTranslation()
                        {
                            LanguageCode = "ar-SA",
                            StageName = "مرحلة التدقيق"
                        }
                    }
                },
                new WorkFlowStepStage()
                {
                    Id = Guid.Parse("5406436a-9d9e-4996-81c6-87cc5e13a653"),
                    StageName = "مراحل دراسة الطلب",
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    IsDeleted = false,
                    Translations = new List<WorkFlowStepStageTranslation>()
                    {
                        new WorkFlowStepStageTranslation()
                        {
                            LanguageCode = "en-US",
                            StageName = "Request Study Stages"
                        },
                        new WorkFlowStepStageTranslation()
                        {
                            LanguageCode = "ar-SA",
                            StageName = "مراحل دراسة الطلب",
                        }
                    }
                }
            };


        }
        #endregion

        #region WorkFlow Steps Actor Type
        private static List<WorkFlowStepsActorType> GetWorkFlowStepsActorTypes()
        {
            return new List<WorkFlowStepsActorType> {
                new WorkFlowStepsActorType()
                {
                    Id = StepActorType.Employee,
                    ActorType = "موظف",
                    Translations = new List<WorkFlowStepsActorTypeTranslation>()
                    {
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            ActorType = "Employee"
                        },
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            ActorType = "موظف"
                        }
                    }
                },
                new WorkFlowStepsActorType()
                {
                    Id = StepActorType.Admin,
                    ActorType = "مدير إدارة",
                    Translations = new List<WorkFlowStepsActorTypeTranslation>()
                    {
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            ActorType = "Department Adminstrator"
                        },
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            ActorType = "مدير إدارة"
                        }
                    }
                },
                new WorkFlowStepsActorType()
                {
                    Id = StepActorType.Official,
                    ActorType = "مسئول",
                    Translations = new List<WorkFlowStepsActorTypeTranslation>()
                    {
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            ActorType = "Official"
                        },
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            ActorType = "مسئول"
                        }
                    }
                },
                new WorkFlowStepsActorType()
                {
                    Id = StepActorType.Department,
                    ActorType = "قسم",
                    Translations = new List<WorkFlowStepsActorTypeTranslation>()
                    {
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            ActorType = "Departmnet"
                        },
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            ActorType = "قسم"
                        }
                    }
                },
                new WorkFlowStepsActorType()
                {
                    Id = StepActorType.Client,
                    ActorType = "عميل",
                    Translations = new List<WorkFlowStepsActorTypeTranslation>()
                    {
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            ActorType = "Client"
                        },
                        new WorkFlowStepsActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            ActorType = "عميل"
                        }
                    }
                }
            };
        }
        #endregion

        #region WorkFlow Step Event Security Level Actor Type
        private static List<WorkFlowStepEventSecurityLevelActorType> GetWorkFlowStepEventSecurityLevelActorTypes()
        {
            return new List<WorkFlowStepEventSecurityLevelActorType> {
                new WorkFlowStepEventSecurityLevelActorType()
                {
                    Id = SecurityLevelActorType.Employee,
                    SecurityLevelActorType = "موظف",
                    Translations = new List<WorkFlowStepEventSecurityLevelActorTypeTranslation>()
                    {
                        new WorkFlowStepEventSecurityLevelActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            SecurityLevelActorType = "Employee"
                        },
                        new WorkFlowStepEventSecurityLevelActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            SecurityLevelActorType = "موظف"
                        }
                    }
                },
                new WorkFlowStepEventSecurityLevelActorType()
                {
                    Id = SecurityLevelActorType.Admin,
                    SecurityLevelActorType = "مدير إدارة",
                    Translations = new List<WorkFlowStepEventSecurityLevelActorTypeTranslation>()
                    {
                        new WorkFlowStepEventSecurityLevelActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            SecurityLevelActorType = "Department Adminstrator"
                        },
                        new WorkFlowStepEventSecurityLevelActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            SecurityLevelActorType = "مدير إدارة"
                        }
                    }
                },
                new WorkFlowStepEventSecurityLevelActorType()
                {
                    Id = SecurityLevelActorType.Official,
                    SecurityLevelActorType = "مسئول",
                    Translations = new List<WorkFlowStepEventSecurityLevelActorTypeTranslation>()
                    {
                        new WorkFlowStepEventSecurityLevelActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            SecurityLevelActorType = "Official"
                        },
                        new WorkFlowStepEventSecurityLevelActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            SecurityLevelActorType = "مسئول"
                        }
                    }
                },
                new WorkFlowStepEventSecurityLevelActorType()
                {
                    Id = SecurityLevelActorType.Department,
                    SecurityLevelActorType  = "قسم",
                    Translations = new List<WorkFlowStepEventSecurityLevelActorTypeTranslation>()
                    {
                        new WorkFlowStepEventSecurityLevelActorTypeTranslation()
                        {
                            LanguageCode = "en-US",
                            SecurityLevelActorType = "Departmnet"
                        },
                        new WorkFlowStepEventSecurityLevelActorTypeTranslation()
                        {
                            LanguageCode = "ar-SA",
                            SecurityLevelActorType = "قسم"
                        }
                    }
                }
            };
        }
        #endregion

        #region WorkFlow Notification Plan
        private static List<WorkFlowNotificationPlan> GetWorkFlowNotificationPlans()
        {
            return new List<WorkFlowNotificationPlan> {
                new WorkFlowNotificationPlan()
                {
                    Id = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                    NotificationPlan = "خطة التنبيه",
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    Translations = new List<WorkFlowNotificationPlanTranslation>()
                    {
                        new WorkFlowNotificationPlanTranslation()
                        {
                            LanguageCode = "en-US",
                            NotificationPlan = "Notification Plan"
                        },
                        new WorkFlowNotificationPlanTranslation()
                        {
                            LanguageCode = "ar-SA",
                            NotificationPlan = "خطة التنيه",
                        }
                    }
                }
            };
        }
        #endregion

        #region WorkFlow Steps Actor Type
        private static List<WorkFlow> GetWorkFlows()
        {
            return new List<WorkFlow>()
            {
                // صلح
                // صلح وتحكيم
                new WorkFlow()
                {
                    Id = Guid.Parse("15d87d0d-3cf4-4969-9c7e-933aeea0e5f5"),
                    WorkFlowName = "دعوة صلح",
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    IsDeleted = false,
                    Translations = new List<WorkFlowTranslation>()
                    {
                        new WorkFlowTranslation()
                        {
                            LanguageCode = "ar-SA",
                            WorkFlowName = "دعوى صلح"
                        },
                        new WorkFlowTranslation()
                        {
                            LanguageCode = "en-US",
                            WorkFlowName = "Conciliation Lititgation"
                        }
                    },
                    WorkFlowSteps = new List<WorkFlowStep>()
                    {
                        new WorkFlowStep()
                        {
                            StepName = "معالجة الملاحظات",
                            ProcessActorType = ProcessActorType.Plaintiff,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("2a15b0d6-1f45-44f2-ba26-01c700b39203"),
                            Order = 0,
                            ActorTypeId = StepActorType.Client,
                            CreationLogMessage = "جارى معالجة الملاحظات",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            //CreationNotificationPlanId = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("88b70280-7c6a-48e4-bf20-66ba537c580b"),
                                    EventName = "إرسال طلب الدعوى",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.Edited,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.Edited,
                                    AllowCompletionNotificationPlan = true,
                                    AllowFileUpload = true,
                                    AllowChangeIsActive = true,
                                    IsActiveValue = true,
                                    AllowNotes = true,
                                    IsAdminApprovalRequired = false,
                                    SameEmployee = true,
                                    AllowChangeIsFinished = true,
                                    IsFinishedValue = false,
                                    AllowChangeIsAllowEdit = true,
                                    IsAllowEditValue = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم معالجة الملاحظات وإرسال الدعوى",
                                    ActionModeId = EventActionMode.GoPreviousStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إرسال طلب الدعوى",
                                            LogMessage = "تم معالجة الملاحظات وإرسال الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Send Litigation Request",
                                            LogMessage = "Feedback has been processed and the request has been sent",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "معالجة الملاحظات",
                                    CreationLogMessage = "جارى معالجة الملاحظات"
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Processing Notes",
                                    CreationLogMessage = "Feedback is being processed"
                                }
                            }
                        }, // تعديل الدعوى
                        new WorkFlowStep()
                        {
                            StepName = "قرار طلب المشارطة",
                            ProcessActorType = ProcessActorType.PlaintiffAndDefendant,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("d443b2ba-64a1-4eb7-b053-4267577be668"),
                            Order = 0,
                            ActorTypeId = StepActorType.Client,
                            CreationLogMessage = "جارى الرد على طلب المشارطة",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            //CreationNotificationPlanId = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "معالجة الملاحظات",
                                    CreationLogMessage = "جارى معالجة الملاحظات"
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Processing Notes",
                                    CreationLogMessage = "Feedback is being processed"
                                }
                            }
                        }, // طلب المشارطة
                        new WorkFlowStep()
                        {
                            StepName = "دفع رسوم الدعوى",

                            ProcessActorType = ProcessActorType.Employee,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("38108bef-1c1c-4b26-9ced-4f25df2235b6"),
                            Order = 1,
                            ActorTypeId = StepActorType.Department,
                            CreationLogMessage = "تم إنشاء الدعوى وجارى إرسال طلب دفع رسوم الدعوى",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,

                            DepartmentId = Guid.Parse("657004f4-57e1-4715-addc-0e5ff0b00938"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("31a0e225-5bd5-430d-a73c-1126f1025156"),
                                    EventName = "إرسال طلب تحصيل رسوم الدعوى",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent,
                                    AllowFileUpload = false,
                                    AllowNotes = false,
                                    AllowChangeIsActive = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم ارسال طلب تحصيل رسوم الدعوى",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    NextWorkFlowStepDestinationId = Guid.Parse("bba119cf-187e-4513-bba6-853506767bbc"),
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إرسال طلب تحصيل رسوم الدعوى",
                                            LogMessage = "تم ارسال طلب تحصيل رسوم الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Send Fees Request",
                                            LogMessage = "Fee collection request has been sent.",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Pay fees",
                                    CreationLogMessage = "The case has been created and the fee payment request is being sent."
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دفع رسوم الدعوى",
                                    CreationLogMessage = "تم إنشاء الدعوى وجارى إرسال طلب دفع رسوم الدعوى",
                                }
                            }
                        }, // ارسال طلب دفع رسوم الدعوة

                        new WorkFlowStep()
                        {
                            StepName = "دفع رسوم الدعوى",
                            ProcessActorType = ProcessActorType.Plaintiff,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("bba119cf-187e-4513-bba6-853506767bbc"),
                            Order = 0,
                            ActorTypeId = StepActorType.Client,
                            CreationLogMessage = "جارى دفع رسوم الدعوى",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            //CreationNotificationPlanId = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("55d4546d-4daa-48b2-ba7f-5534f7e87b1b"),
                                    EventName = "تم دفع رسوم الدعوى",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected,
                                    AllowCompletionNotificationPlan = true,
                                    AllowFileUpload = false,
                                    AllowChangeIsActive = true,
                                    IsActiveValue = true,
                                    AllowNotes = false,
                                    IsAdminApprovalRequired = false,
                                    SameEmployee = true,
                                    AllowChangeIsFinished = true,
                                    IsFinishedValue = false,
                                    AllowChangeIsAllowEdit = true,
                                    IsAllowEditValue = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم دفع رسوم الدعوى وإرسال الدعوى",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    NextWorkFlowStepDestinationId = Guid.Parse("15154695-a36b-4fda-8572-4874f661000e"),
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "تم دفع رسوم الدعوى",
                                            LogMessage = "تم دفع رسوم الدعوى وإرسال الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Fees paid",
                                            LogMessage = "Fees paid and claim sent",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دفع رسوم الدعوى",
                                    CreationLogMessage = "جارى دفع رسوم الدعوى",
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Pay Fees",
                                    CreationLogMessage = "Fees are being paid"
                                }
                            }
                        }, // دفع رسوم الدعوى
                       
                        new WorkFlowStep()
                        {
                            StepName = "دفع رسوم الصلح",

                            ProcessActorType = ProcessActorType.Employee,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("80ffddb4-1a64-4533-b02f-1af99675b774"),
                            Order = 3,
                            ActorTypeId = StepActorType.Department,
                            CreationLogMessage = "تم قبول الدعوى وجارى إرسال طلب دفع رسوم الصلح",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                           
                            //CreationNotificationPlanId = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                            DepartmentId = Guid.Parse("657004f4-57e1-4715-addc-0e5ff0b00938"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("0acdfc5f-7d2b-4b8c-b121-5faf14b0feea"),
                                    EventName = "إرسال طلب تحصيل رسوم الصلح",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.CollectionOfReconciliationFeesRequestIsBeingSent,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.CollectionOfReconciliationFeesRequestIsBeingSent,
                                    AllowFileUpload = false,
                                    AllowNotes = false,
                                    AllowChangeIsActive = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم ارسال طلب تحصيل رسوم الصلح",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    NextWorkFlowStepDestinationId = Guid.Parse("f0e28556-e0ab-4b7e-97f1-8a9912d720a3"),
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إرسال طلب تحصيل رسوم الصلح",
                                            LogMessage = "تم ارسال طلب تحصيل رسوم الصلح",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Send Fees Request",
                                            LogMessage = "Fee collection reconciliation has been sent.",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Pay fees",
                                    CreationLogMessage = "The case has been created and the fee payment request is being sent."
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دفع رسوم الدعوى",
                                    CreationLogMessage = "تم قبول الدعوى وجارى إرسال طلب دفع رسوم الصلح",
                                }
                            }
                        }, // ارسال طلب دفع رسوم الصلح
                        new WorkFlowStep()
                        {
                            StepName = "دفع رسوم الصلح",
                            ProcessActorType = ProcessActorType.Plaintiff,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("f0e28556-e0ab-4b7e-97f1-8a9912d720a3"),
                            Order = 0,
                            ActorTypeId = StepActorType.Client,
                            CreationLogMessage = "جارى دفع رسوم الصلح",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("8d908df8-c897-4cd3-94cf-b8f5cd218f9f"),
                                    EventName = "تم دفع رسوم الصلح",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.ReconciliationFeesHaveBeenCollected,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.ReconciliationFeesHaveBeenCollected,
                                    LitigationRequestStage = LitigationRequestStage.Reconciliation,
                                    LitigationRequestSubStage = LitigationRequestSubStage.AddSession,
                                    AllowCompletionNotificationPlan = true,
                                    AllowFileUpload = false,
                                    AllowChangeIsActive = true,
                                    IsActiveValue = true,
                                    AllowNotes = false,
                                    IsAdminApprovalRequired = false,
                                    SameEmployee = true,
                                    AllowChangeIsFinished = true,
                                    IsFinishedValue = false,
                                    AllowChangeIsAllowEdit = true,
                                    IsAllowEditValue = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم دفع رسوم الدعوى وإرسال الدعوى",
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    ActionModeId = EventActionMode.SpecificStep,
                                    NextWorkFlowStepDestinationId = Guid.Parse("01c85df6-ee7c-4265-8544-65744910a941"),
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "تم دفع رسوم الصلح",
                                            LogMessage = "تم دفع رسوم الصلح وإرسال الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Fees paid",
                                            LogMessage = "Fees paid and claim sent",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دفع رسوم الدعوى",
                                    CreationLogMessage = "جارى دفع رسوم الدعوى",
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Pay Fees",
                                    CreationLogMessage = "Fees are being paid"
                                }
                            }
                        }, // دفع رسوم الصلح
                       
                        new WorkFlowStep()
                        {
                            StepName = "تدقيق الدعوى",

                            ProcessActorType = ProcessActorType.Employee,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("15154695-a36b-4fda-8572-4874f661000e"),
                            Order = 2,
                            ActorTypeId = StepActorType.Department,
                            CreationLogMessage = "تم إنشاء الدعوى وجارى تدقيقها",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            InternalStatusId = RequestStatus.New,
                            ExternalStatusId = RequestStatus.New,

                           
                            //CreationNotificationPlanId = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                            DepartmentId = Guid.Parse("657004f4-57e1-4715-addc-0e5ff0b00938"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("3f01924e-59f4-44b7-9506-75c85b89b773"),
                                    EventName = "قبول",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.LitigationApprovedCollectionOfReconciliationFeesRequestIsBeingSend,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.LitigationApprovedCollectionOfReconciliationFeesRequestIsBeingSend,
                                    AllowFileUpload = false,
                                    IsActiveValue = false,
                                    AllowChangeIsFinished = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    AllowNotes = false,
                                    IsAdminApprovalRequired = false,
                                    ActionModeId = EventActionMode.SpecificStep,
                                    NextWorkFlowStepDestinationId = Guid.Parse("80ffddb4-1a64-4533-b02f-1af99675b774"),
                                    LogMessage = "تم قبول الدعوى",
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    SameEmployee = false,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "قبول",
                                            LogMessage = "تم قبول الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Accept",
                                            LogMessage = "Litigation has been accepted",
                                        }
                                    }
                                },
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("8d305628-46f7-45da-a177-77274c06d433"),
                                    EventName = "رفض",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.Rejected,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.Rejected,
                                    AllowCompletionNotificationPlan = true,
                                    //CompletionNotificationPlanId = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                                    AllowFileUpload = false,
                                    IsAdminApprovalRequired = false,
                                    AllowChangeIsActive = true,
                                    AllowChangeIsFinished = true,
                                    IsFinishedValue = true,
                                    IsActiveValue = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    SameEmployee = false,
                                    AllowNotes = true,
                                    LogMessage = "تم رفض الدعوى",
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = true,
                                    IsAllowEditValue = true,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "رفض",
                                            LogMessage = "تم رفض الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Reject",
                                            LogMessage = "Request has been Rejected",
                                        }
                                    }
                                },
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("8a392baf-a959-429a-b2c9-eea05054ff11"),
                                    EventName = "إعادة الدعوى للتعديل",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.SentBackForEdit,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.SentBackForEdit,
                                    AllowFileUpload = true,
                                    AllowChangeIsActive = false,
                                    AllowNotes = true,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم إعادة الدعوى للتعديل",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    NextWorkFlowStepDestinationId = Guid.Parse("2a15b0d6-1f45-44f2-ba26-01c700b39203"),
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إعادة  الدعوى للتعديل",
                                            LogMessage = "تم دراسة الدعوى وإعادتها للتعديل",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Send Back For Edit",
                                            LogMessage = "Request was studied and sent for edit",
                                        }
                                    }
                                },
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("40b92a76-2c2d-4c32-a0f9-0c8bb03a67a9"),
                                    EventName = "إرسال طلب مشارطة",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.AgreementRequestHasBeenSent,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.AgreementRequestHasBeenSent,
                                    AllowChangeIsActive = false,
                                    AllowNotes = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم إرسال طلب مشارطة",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إرسال طلب مشارطة",
                                            LogMessage = "تم إرسال طلب مشارطة",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Charter Request Sent",
                                            LogMessage = "Agreement Request Has Been Sent",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Request Studing",
                                    CreationLogMessage = "Litigation has been created and is being reviewed"
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دراسة الدعوى",
                                    CreationLogMessage = "تم إنشاء الدعوى وجارى تدقيقها",
                                }
                            }
                        }, //تدقيق الدعوى

                        new WorkFlowStep()
                        {
                            StepName = "تأكيد دفع رسوم الصلح",

                            ProcessActorType = ProcessActorType.Employee,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("01c85df6-ee7c-4265-8544-65744910a941"),
                            Order = 3,
                            ActorTypeId = StepActorType.Department,
                            CreationLogMessage = "تم تأكيد دفع رسوم الصلح",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            DepartmentId = Guid.Parse("657004f4-57e1-4715-addc-0e5ff0b00938"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("1783b541-f8a4-43a4-93ef-bab280a78c57"),
                                    EventName = "تأكيد دفع رسوم الصلح",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.Received,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.Received,
                                    AllowFileUpload = false,
                                    AllowNotes = false,
                                    AllowChangeIsActive = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم تأكيد تحصيل رسوم الصلح",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "تأكيد دفع رسوم الصلح",
                                            LogMessage = "تم تأكيد تحصيل رسوم الصلح",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Confirm Reconciliation Fees Collection",
                                            LogMessage = "Reconciliation Fees Collected.",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Pay fees",
                                    CreationLogMessage = "The case has been created and the fee payment request is being sent."
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دفع رسوم الدعوى",
                                    CreationLogMessage = "تم قبول الدعوى وجارى إرسال طلب دفع رسوم الصلح",
                                }
                            }
                        }, // تأكيد دفع رسوم الصلح


                    }
                },

                // تجكيم
                new WorkFlow()
                {
                    Id = Guid.Parse("8efe7ba7-25fa-4dd7-a299-e4744a81e217"),
                    WorkFlowName = "دعوة تحكيم",
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    IsDeleted = false,
                    Translations = new List<WorkFlowTranslation>()
                    {
                        new WorkFlowTranslation()
                        {
                            LanguageCode = "ar-SA",
                            WorkFlowName = "دعوى تحكيم"
                        },
                        new WorkFlowTranslation()
                        {
                            LanguageCode = "en-US",
                            WorkFlowName = "Arbitration Lititgation"
                        }
                    },
                    WorkFlowSteps = new List<WorkFlowStep>()
                    {
                        new WorkFlowStep()
                        {
                            StepName = "معالجة الملاحظات",
                            ProcessActorType = ProcessActorType.Plaintiff,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("472e009a-9bda-41ae-9718-4fac826ad6e0"),
                            Order = 0,
                            ActorTypeId = StepActorType.Client,
                            CreationLogMessage = "جارى معالجة الملاحظات",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            //CreationNotificationPlanId = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("62c26d61-b150-4862-81b9-398b28b050e5"),
                                    EventName = "إرسال طلب الدعوى",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.Edited,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.Edited,
                                    AllowCompletionNotificationPlan = true,
                                    AllowFileUpload = true,
                                    AllowChangeIsActive = true,
                                    IsActiveValue = true,
                                    AllowNotes = true,
                                    IsAdminApprovalRequired = false,
                                    SameEmployee = true,
                                    AllowChangeIsFinished = true,
                                    IsFinishedValue = false,
                                    AllowChangeIsAllowEdit = true,
                                    IsAllowEditValue = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم معالجة الملاحظات وإرسال الدعوى",
                                    ActionModeId = EventActionMode.GoPreviousStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إرسال طلب الدعوى",
                                            LogMessage = "تم معالجة الملاحظات وإرسال الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Send Litigation Request",
                                            LogMessage = "Feedback has been processed and the request has been sent",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "معالجة الملاحظات",
                                    CreationLogMessage = "جارى معالجة الملاحظات"
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Processing Notes",
                                    CreationLogMessage = "Feedback is being processed"
                                }
                            }
                        }, // تعديل الدعوى

                        new WorkFlowStep()
                        {
                            StepName = "دفع رسوم الدعوى",

                            ProcessActorType = ProcessActorType.Employee,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("069af4c4-f24a-471b-b7d3-b2ffbeeac960"),
                            Order = 1,
                            ActorTypeId = StepActorType.Department,
                            CreationLogMessage = "تم إنشاء الدعوى وجارى إرسال طلب دفع رسوم الدعوى",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,

                            DepartmentId = Guid.Parse("657004f4-57e1-4715-addc-0e5ff0b00938"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("56a84136-8026-471b-8dcc-38de04865811"),
                                    EventName = "إرسال طلب تحصيل رسوم الدعوى",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent,
                                    AllowFileUpload = false,
                                    AllowNotes = false,
                                    AllowChangeIsActive = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم ارسال طلب تحصيل رسوم الدعوى",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    NextWorkFlowStepDestinationId = Guid.Parse("4332736f-2acf-49da-9294-ff31233a7571"),
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إرسال طلب تحصيل رسوم الدعوى",
                                            LogMessage = "تم ارسال طلب تحصيل رسوم الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Send Fees Request",
                                            LogMessage = "Fee collection request has been sent.",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Pay fees",
                                    CreationLogMessage = "The case has been created and the fee payment request is being sent."
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دفع رسوم الدعوى",
                                    CreationLogMessage = "تم إنشاء الدعوى وجارى إرسال طلب دفع رسوم الدعوى",
                                }
                            }
                        }, // ارسال طلب دفع رسوم الدعوة

                        new WorkFlowStep()
                        {
                            StepName = "دفع رسوم الدعوى",
                            ProcessActorType = ProcessActorType.Plaintiff,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("4332736f-2acf-49da-9294-ff31233a7571"),
                            Order = 0,
                            ActorTypeId = StepActorType.Client,
                            CreationLogMessage = "جارى دفع رسوم الدعوى",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            //CreationNotificationPlanId = Guid.Parse("f7c76133-2b18-4238-9081-d1fefbefb7cb"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("c180bfeb-7cf8-4736-b573-0b6ed24fdc38"),
                                    EventName = "تم دفع رسوم الدعوى",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected,
                                    AllowCompletionNotificationPlan = true,
                                    AllowFileUpload = false,
                                    AllowChangeIsActive = true,
                                    IsActiveValue = true,
                                    AllowNotes = false,
                                    IsAdminApprovalRequired = false,
                                    SameEmployee = true,
                                    AllowChangeIsFinished = true,
                                    IsFinishedValue = false,
                                    AllowChangeIsAllowEdit = true,
                                    IsAllowEditValue = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم دفع رسوم الدعوى وإرسال الدعوى",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    NextWorkFlowStepDestinationId = Guid.Parse("dbd6bf09-8169-44ff-bb11-e037db127743"),
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "تم دفع رسوم الدعوى",
                                            LogMessage = "تم دفع رسوم الدعوى وإرسال الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Fees paid",
                                            LogMessage = "Fees paid and claim sent",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دفع رسوم الدعوى",
                                    CreationLogMessage = "جارى دفع رسوم الدعوى",
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Pay Fees",
                                    CreationLogMessage = "Fees are being paid"
                                }
                            }
                        }, // دفع رسوم الدعوى

                        new WorkFlowStep()
                        {
                            StepName = "قرار طلب المشارطة",
                            ProcessActorType = ProcessActorType.PlaintiffAndDefendant,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("d63db9e7-370f-4da6-9e1e-8220047d9b99"),
                            Order = 0,
                            ActorTypeId = StepActorType.Client,
                            CreationLogMessage = "جارى الرد على طلب المشارطة",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "معالجة الملاحظات",
                                    CreationLogMessage = "جارى معالجة الملاحظات"
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Processing Notes",
                                    CreationLogMessage = "Feedback is being processed"
                                }
                            }
                        }, // طلب المشارطة
                        new WorkFlowStep()
                        {
                            StepName = "تدقيق الدعوى",

                            ProcessActorType = ProcessActorType.Employee,
                            WorkModeId = StepWorkMode.None,
                            Id = Guid.Parse("dbd6bf09-8169-44ff-bb11-e037db127743"),
                            Order = 2,
                            ActorTypeId = StepActorType.Department,
                            CreationLogMessage = "تم إنشاء الدعوى وجارى تدقيقها",
                            CreatedOn = DateTime.Now,
                            CreatedBy = Guid.NewGuid(),
                            IsDeleted = false,
                            InternalStatusId = RequestStatus.New,
                            ExternalStatusId = RequestStatus.New,
                            DepartmentId = Guid.Parse("657004f4-57e1-4715-addc-0e5ff0b00938"),
                            StageId = _context.WorkFlowStepStages.ToList().FirstOrDefault().Id,
                            ShowForApplicant = true,
                            WorkFlowStepEvents = new List<WorkFlowStepEvent>()
                            {
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("fb7681be-85de-4855-9c3b-e466262c1de6"),
                                    EventName = "قبول",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.Received,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.Received,
                                    AllowFileUpload = false,
                                    IsActiveValue = false,
                                    AllowChangeIsFinished = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    AllowNotes = false,
                                    IsAdminApprovalRequired = false,
                                    LogMessage = "تم قبول الدعوى",
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    SameEmployee = false,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "قبول",
                                            LogMessage = "تم قبول الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Accept",
                                            LogMessage = "Litigation has been accepted",
                                        }
                                    }
                                },
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("0672ea48-76a9-49ee-9500-63ef5187f067"),
                                    EventName = "رفض",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.Rejected,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.Rejected,
                                    AllowCompletionNotificationPlan = true,
                                    AllowFileUpload = false,
                                    IsAdminApprovalRequired = false,
                                    AllowChangeIsActive = true,
                                    AllowChangeIsFinished = true,
                                    IsFinishedValue = true,
                                    IsActiveValue = false,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    SameEmployee = false,
                                    AllowNotes = true,
                                    LogMessage = "تم رفض الدعوى",
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = true,
                                    IsAllowEditValue = true,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "رفض",
                                            LogMessage = "تم رفض الدعوى",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Reject",
                                            LogMessage = "Request has been Rejected",
                                        }
                                    }
                                },
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("f2c83a0e-ac1b-43ec-820e-098632896fdd"),
                                    EventName = "إعادة الدعوى للتعديل",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.SentBackForEdit,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.SentBackForEdit,
                                    AllowFileUpload = true,
                                    AllowChangeIsActive = false,
                                    AllowNotes = true,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم إعادة الدعوى للتعديل",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    NextWorkFlowStepDestinationId = Guid.Parse("472e009a-9bda-41ae-9718-4fac826ad6e0"),
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إعادة  الدعوى للتعديل",
                                            LogMessage = "تم دراسة الدعوى وإعادتها للتعديل",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Send Back For Edit",
                                            LogMessage = "Request was studied and sent for edit",
                                        }
                                    }
                                },
                                new WorkFlowStepEvent()
                                {
                                    Id = Guid.Parse("5ff2c282-23ef-4f7e-857a-ce06ee9390ca"),
                                    EventName = "إرسال طلب مشارطة",
                                    AllowChangeInternalStatus = true,
                                    InternalStatusId = RequestStatus.AgreementRequestHasBeenSent,
                                    AllowChangeExternalStatus = true,
                                    ExternalStatusId = RequestStatus.AgreementRequestHasBeenSent,
                                    AllowChangeIsActive = false,
                                    AllowNotes = true,
                                    EventSecurityLevelId = EventSecurityLevel.UnSpecified,
                                    LogMessage = "تم إرسال طلب مشارطة",
                                    ActionModeId = EventActionMode.SpecificStep,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = Guid.NewGuid(),
                                    IsDeleted = false,
                                    ShowLogForApplicant = true,
                                    ShowProcessingLogForApplicant = true,
                                    Translations  = new List<WorkFlowStepEventTranslation>()
                                    {
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "ar-SA",
                                            EventName = "إرسال طلب مشارطة",
                                            LogMessage = "تم إرسال طلب مشارطة",
                                        },
                                        new WorkFlowStepEventTranslation()
                                        {
                                            LanguageCode = "en-US",
                                            EventName = "Charter Request Sent",
                                            LogMessage = "Agreement Request Has Been Sent",
                                        }
                                    }
                                }
                            },
                            Translations = new List<WorkFlowStepTranslation>()
                            {
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "en-US",
                                    StepName = "Request Studing",
                                    CreationLogMessage = "Litigation has been created and is being reviewed"
                                },
                                new WorkFlowStepTranslation()
                                {
                                    LanguageCode = "ar-SA",
                                    StepName = "دراسة الدعوى",
                                    CreationLogMessage = "تم إنشاء الدعوى وجارى تدقيقها",
                                }
                            }
                        }, //تدقيق الدعوى
                    }
                },



            };
        }
        #endregion
    }
}
