namespace REAC.Domain.Shared.Enum
{
    public enum RequestType
    {
        // طلبات المهام - الصلح
        // LitigationRequestReformerAssigningRequest Id
        ReformerTaskrequests = 1,
        // جلسات الصلح
        // LitigationRequest Id
        ReformationSession = 2,
        // محضر الصلح
        // LitigationRequest Id
        ReformationReport = 3,
        // طلب مد مدة الصلح
        // LitigationRequest Id
        ExtendReconciliationPeriodRequest = 4,
        // ايقاف وتحرير مصلح
        // Citizen Id
        StopFreeUpReformer = 5,
        // طلبات المهام - التحكيم
        // LitigationRequestArbitratorAssigningRequest Id
        ArbitratorTaskrequests = 6,
        // جلسات التحكيم
        // LitigationRequest Id
        ArbitrationSession = 7,
        // حكم التحكيم
        // LitigationRequest Id
        ArbitrationReport = 8,
        // طلب مد مدة التحكيم
        // LitigationRequest Id
        ExtendArbitrationPeriodRequest = 9,
        // ايقاف وتحرير محكم
        // Citizen Id
        StopFreeUpArbitrator = 10,
        // الدعاوى
        // LitigationRequest Id
        LitigationRequest = 11,
        // طلب الانضام كمصلح
        // CitizenRequest Id
        JoinRequestAsReformerRequest = 12,
        // طلب الانضام كمحكم
        // CitizenRequest Id
        JoinRequestAsArbitratorRequest = 13,

        // قائمة ترشيح هيئة التحكيم
        // LitigationRequest Id
        ArbitratorNominationList = 14,

        // قائمة ترشيح رئيس هيئة التحكيم
        // LitigationRequest Id
        ArbitratorChairmanNominationList = 15,
        
        // اضافة وكيل
        // Agent
        AddAgent = 16,

        //  اضافة محامي 
        // Lawyer
        AddLawyer = 17,
        
        //  حذف محامي 
        // Lawyer
        DeleteLawyer = 18,
        
        //   طلبات المشارطه
        // Litigation Agreement
        LitigationAgreementRequest = 19,
        
        //   طلبات المشارطه
        // Litigation Agreement
        LitigationRequestCorrespondence = 20,

        //   طلبات المشارطه
        // Litigation Agreement
        LitigationRequestCorrespondenceReply = 21,
    }
}
