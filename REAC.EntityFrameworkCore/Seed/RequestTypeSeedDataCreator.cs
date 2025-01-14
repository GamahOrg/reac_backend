using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Notification;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public partial class RequestTypeSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysRequestTypes.AnyAsync()) return;
            await context.SysRequestTypes.AddRangeAsync(new List<SysRequestType>()
            {
                new SysRequestType
                {
                    Id = RequestType.ReformerTaskrequests,
                    RequestTypeName = "طلبات المهام - الصلح",
                },
                new SysRequestType
                {
                    Id = RequestType.ReformationSession,
                    RequestTypeName = "جلسات الصلح",
                },
                new SysRequestType
                {
                    Id = RequestType.ReformationReport,
                    RequestTypeName = "محاضر الصلح",
                },
                new SysRequestType
                {
                    Id = RequestType.StopFreeUpReformer,
                    RequestTypeName = "قرار الإيقاف والتحرير لمصلح",
                },
                new SysRequestType
                {
                    Id = RequestType.ExtendReconciliationPeriodRequest,
                    RequestTypeName = "طلب مد مدة الصلح",
                },
                new SysRequestType
                {
                    Id = RequestType.ArbitratorTaskrequests,
                    RequestTypeName = "طلبات المهام - التحكيم",
                },
                new SysRequestType
                {
                    Id = RequestType.ArbitrationSession,
                    RequestTypeName = "جلسات التحكيم",
                },
                new SysRequestType
                {
                    Id = RequestType.ArbitrationReport,
                    RequestTypeName = "أحكام التحكيم",
                },
                new SysRequestType
                {
                    Id = RequestType.StopFreeUpArbitrator,
                    RequestTypeName = "قرار الإيقاف والتحرير لمحكم",
                },
                new SysRequestType
                {
                    Id = RequestType.ExtendArbitrationPeriodRequest,
                    RequestTypeName = "طلب مد مدة التحكيم",
                },
                new SysRequestType
                {
                    Id = RequestType.LitigationRequest,
                    RequestTypeName = "طلب دعوى",
                },
                new SysRequestType
                {
                    Id = RequestType.JoinRequestAsReformerRequest,
                    RequestTypeName = "طلب الإنضمام كمصلح",
                },
                new SysRequestType
                {
                    Id = RequestType.JoinRequestAsArbitratorRequest,
                    RequestTypeName = "طلب الإنضمام كمحكم",
                },
                new SysRequestType
                {
                    Id = RequestType.ArbitratorNominationList,
                    RequestTypeName = "قائمة ترشيح هيئة التحكيم",
                },
                new SysRequestType
                {
                    Id = RequestType.ArbitratorChairmanNominationList,
                    RequestTypeName = "قائمة ترشيح رئيس هيئة التحكيم",
                },
                new SysRequestType
                {
                    Id = RequestType.AddAgent,
                    RequestTypeName = "اضافة وكيل",
                },
                new SysRequestType
                {
                    Id = RequestType.AddLawyer,
                    RequestTypeName = "اضافة محامي",
                },
                new SysRequestType
                {
                    Id = RequestType.DeleteLawyer,
                    RequestTypeName = "حذف محامي",
                },new SysRequestType
                {
                    Id = RequestType.LitigationAgreementRequest,
                    RequestTypeName = "طلب المشارطة",
                },new SysRequestType
                {
                    Id = RequestType.LitigationRequestCorrespondence,
                    RequestTypeName = "المراسلات",
                },new SysRequestType
                {
                    Id = RequestType.LitigationRequestCorrespondenceReply,
                    RequestTypeName = "رد المراسلات",
                }


            });
            context.SaveChanges();
        }
    }
}