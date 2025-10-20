using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class SettingSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysSettings.AnyAsync()) return;
            await context.SysSettings.AddRangeAsync(
                new List<SysSetting>
                {
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "REACPortalUrl",
                        Value = "https://dev-reac-backoffice.rega.gov.sa/",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SecurityApiUrl",
                        Value = "http://localhost:7144/",
                    },

                    #region Complaints Integration
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GetRegionList",
                        Value = "http://localhost:7144/v1/rega/crm/module/fields?ModuleName=Cases&fields=rejon_c",
                    }, new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GetCaseById",
                        Value = "http://localhost:7144/v1/rega/crm/entry/list?ModuleName=Cases&QueryField=cases.id&QueryFieldValue={caseId}&SelectFields=id,rejon_c,requestclassification_c,program_c,reference_number_c,request_number_c,name,description,date_entered,case_number,status",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GetContact",
                        Value = "http://localhost:7144/v1/rega/crm/entry/list?ModuleName=Contacts&QueryField=national_id_c&QueryFieldValue={nationalId}&SelectFields=id,first_name,last_name,email1,national_id_c,phone_mobile",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GetCaseById",
                        Value = "http://localhost:7144/v1/rega/crm/entry/list?ModuleName=Cases&QueryField=cases.id&QueryFieldValue={CaseId}&SelectFields=id,rejon_c,requestclassification_c,program_c,reference_number_c,request_number_c,name,description,date_entered,case_number,status",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GetCasesByContact",
                        Value = "http://localhost:7144/v1/rega/crm/entry/relationship?ModuleName=Contacts&ModuleId={ModuleId}&RelatedModuleQuery=&Deleted=0&RelatedFields=id,case_number,name,status,date_entered&RelatedModuleLinkName=&LinkFieldName=cases",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "CreateNewNote",
                        Value = "http://localhost:7144/v1/rega/crm/entry",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "UploadFileToNote",
                        Value = "http://localhost:7144/v1/rega/crm/note-attachment",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "CreateContact",
                        Value = "http://localhost:7144/v1/rega/crm/entry",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "CreateNewCase",
                        Value = "http://localhost:7144/v1/rega/crm/entry",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LinkNoteToCase",
                        Value = "http://localhost:7144/v1/rega/crm/relationship",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LinkContactToCase",
                        Value = "http://localhost:7144/v1/rega/crm/relationship",
                    },
	                #endregion

                    #region DelegationIntegration
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "DelegationValidation",
                        Value = "https//localhost:4177/v1/moj/external-attorney/GetAttorneys?request.attorneyCode={attorneyCode}&request.identityNumber={identityNumber}",
                    },
	                #endregion

                    #region GEE Login Integration

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GEE_client_id",
                        Value = "UGgZBIiwnTm0KqwerVrAkcblxWnpYc1l",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GEE_client_secret",
                        Value = "g33z5iKMO3BWpcAe",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GEE_grant_type",
                        Value = "client_credentials",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GEE_LoginURL",
                        Value = "https://api-test.rega.gov.sa/v1/oauth",
                    },

                    #endregion

                    #region MOJ

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "MOJ_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/moj/external-attorney/GetAttorneys?request.attorneyCode={0}&request.identityNumber={1}",
                    },

                    #endregion

                    #region Nafath

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Nafath_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/iam/nafath",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Nafath_Action",
                        Value = "SpRequest",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Nafath_Service",
                        Value = "AdvancedLogin",
                    },

                    #endregion

                    #region Nafath Check

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "NafathCheck_Action",
                        Value = "CheckSpRequest",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "NafathCheck_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/iam/nafath",
                    },
                    
                    #endregion

                    #region Nafath Get User Info

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "NafathGetUserInfo_URL",
                        Value = "https://api-test.rega.gov.sa/v1/iam/identityServices/userInfo",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "GEENafathCheckEnableNafathToken",
                        Value = "false",
                    },

                    #endregion

                    #region Nafath Get User Address Info

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "NafathGetUserAddressInfo_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/spl/addresses/individual?IdentifierType=NationalID&Identifier={0}",
                    },

                    #endregion

                    #region Wafi 
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Wafi_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/wafi/get-project-details-by-license?licenseNumber={0}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "WafiData_URL",
                        Value = "https://wafiservicesapi.rega.gov.sa/api/REAC/GetLicenseProjectPartiesData?licenseNumber={0}&identityNumber={1}",
                    },
	                #endregion

                    #region Ejar
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Ejar_ContractDetails",
                        Value = "https://api-dev.rega.gov.sa/v1/ejar/external/contracts/details?pageNumber=1&pageSize=1&contractNumber={0}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Ejar_ContractFinancialDetails",
                        Value = "https://api-dev.rega.gov.sa/v1/ejar/external/financial-contracts/details?pageNumber=1&pageSize=1&contractNumber={0}",
                    },
	                #endregion

                    #region MC

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "MC_GetCRInformationByCROwners_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/mc/cr-owners?CRNumber={0}",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "MC_GetCRInformationByCRNumber_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/mc/cr-information?CRNumber={0}",
                    },

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "MC_GetCRBasicInfo_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/mc/cr-basic-info",
                    },

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "MC_GetCRPersonRelated_URL",
                        Value = "https://api-dev.rega.gov.sa/v1/mc/person-related-crs",
                    },

                    #endregion

                    #region SMS

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Disable_SMS_Integration",
                        Value = "false",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SMS_URL",
                        Value = "https://api-test.rega.gov.sa/v1/rega/notificationservice/notify",
                    },

                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SMS_Notification_Type",
                        Value = "SMS",
                    }, new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Litigation_Request_Max_Session_ReScheduleTime",
                        Value = "3",
                    },
                    

                    #endregion

                    #region OTP Setting

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "OTP.Lenght",
                        Value = "6",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "OTP.Key",
                        Value = "D6",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "OTP.TimeoutInSeconds",
                        Value = "120", // 2 minutes
                    },

                    #endregion

                    #region
                    

                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitration.ExpirationDateHours",
                        Value = "5",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitration.Period",
                        Value = "30",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Review.Period",
                        Value = "10",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Period",
                        Value = "30",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Request.Study.Period",
                        Value = "3",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Request.Process.Notes.Period",
                        Value = "2",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Reformer.Approve.Mission.Period",
                        Value = "3",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.LitigationParties.Response.Period",
                        Value = "3",
                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),

                        Key = "Reconciliation.Report.Creation",
                        Value = "تم تحرير محضر الصُلح على الدعوى رقم {Litigation.Code}",
                    },
                    #endregion

                    #region NotificationMessages

                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Delegator_Stopped,
                        Value="تم ايقاف المفوض {CitizenName} المقدم نيابة عن {Party} لدعوي رقم {LitigationCode}"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Delegator_Modified,
                        Value="تم تعديل المفوض {CitizenName} المقدم نيابة عن {Party} لدعوي رقم {LitigationCode}"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Delegator_Assigned,
                        Value="تم تعيين المفوض {CitizenName} المقدم نيابة عن {Party} لدعوي رقم {LitigationCode}"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Arbitrator_UpdateJoiningRequest_ToPayFees,
                        Value="تنويه برجاء دفع رسوم الانضمام كمحكم {CitizenName}"
                    },
                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Employee_Arbitrator_UpdateJoiningRequest_ToPayFees,
                        Value="تم ارسال طلب {CitizenName} للانضمام كمحكم لدفع الرسوم"
                    },

                       new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Arbitrator_UpdateJoiningRequest_ToPayEditFees,
                        Value="تنويه برجاء دفع رسوم تعديل طلب الانضمام كمحكم {CitizenName}"
                    },
                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Employee_Arbitrator_UpdateJoiningRequest_ToPayEditFees,
                        Value="تم ارسال طلب {CitizenName} للانضمام كمحكم لدفع رسوم تعديل الطلب"
                    },
                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Reformer_UpdateJoiningRequest_ToPayFees,
                        Value="تنويه برجاء دفع رسوم الانضمام كمصلح {CitizenName}"
                    },
                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key=SysSettingKeyConstants.Employee_Reformer_UpdateJoiningRequest_ToPayFees,
                        Value="تم ارسال طلب {CitizenName} للانضمام كمصلح لدفع الرسوم"
                    },

                     new SysSetting
                     {
                         Id= Guid.NewGuid(),
                         Key=SysSettingKeyConstants.Arbitrator_UpdateJoiningRequest_PaidFees,
                         Value="تم اعادة ارسال طلب الانضمام كمحكم لاستكمال الدراسة"
                     },

                     new SysSetting
                     {
                         Id= Guid.NewGuid(),
                         Key=SysSettingKeyConstants.Employee_Arbitrator_UpdateJoiningRequest_PaidFees,
                         Value="تم دفع رسوم طلب {CitizenName} للانضمام كمحكم "
                     },
                         new SysSetting
                     {
                         Id= Guid.NewGuid(),
                         Key=SysSettingKeyConstants.Employee_Arbitrator_UpdateJoiningRequest_PaidEditFees,
                         Value="تم دفع رسوم طلب تعديل الطلب {CitizenName} للانضمام كمحكم "
                     },
                     new SysSetting
                     {
                         Id= Guid.NewGuid(),
                         Key=SysSettingKeyConstants.Reformer_UpdateJoiningRequest_PaidFees,
                         Value="تم اعادة ارسال طلب الانضمام كمصلح لاستكمال الدراسة"
                     },

                     new SysSetting
                     {
                         Id= Guid.NewGuid(),
                         Key=SysSettingKeyConstants.Employee_Reformer_UpdateJoiningRequest_PaidFees,
                         Value="تم دفع رسوم طلب {CitizenName} للانضمام كمصلح "
                     },


                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Citizen.AddBankAccount",
                        Value = "تم اضافة بيانات الحساب البنكي الخاص ب {CitizenName} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Citizen.AddAgent",
                        Value = "تم اضافة وكيل جديد بأسم  {CitizenName} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Delegator.AddAgent",
                        Value = "تم اضافتك كوكيل للمستخدم {CitizenName} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Citizen.AddLawyer",
                        Value = "تم اضافة محامي جديد بأسم  {CitizenName} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Delegator.AddLawyer",
                        Value = "تم اضافتك كمحامي للمستخدم {CitizenName} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Citizen.DeleteLAwyer",
                        Value = "تم حذف المحامي بأسم  {CitizenName} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Delegator.DeleteLawyer",
                        Value = "تم حذفك كمحامي للمستخدم {CitizenName} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Citizen.EditBankAccount",
                        Value = "تم تعديل بيانات الحساب البنكي الخاص ب {CitizenName} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.JoiningRequest",
                        Value = "تم اضافة طلب جديد بالانضمام كمصلح بأسم {CitizenName} لدى المركز السعودي للتحكيم العقاري",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.JoiningRequest",
                        Value = "تم تقديم طلب بالانضمام كمحكم بأسم {CitizenName} لدى المركز السعودي للتحكيم العقاري",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.AcceptJoiningRequest",
                        Value = "تم قبولكم {CitizenName} كمصلح معتمد لدى المركز السعودي للتحكيم العقاري",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.AcceptJoiningRequest",
                        Value = "تم اعتماد طلب {CitizenName} بالانضمام كمحكم ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.AcceptJoiningRequest.WaitingForFees",
                        Value = "تم اعتماد طلب {CitizenName} للانضمام كمصلح و انتظار دفع رسوم العضوية.",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.AcceptJoiningRequest.WaitingForFees",
                        Value = "تم قبول طلب {CitizenName} كمحكم معتمد لدى المركز السعودي للتحكيم العقاري و انتظار دفع رسوم العضوية.",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.AcceptJoiningRequest.NoFees",
                        Value = "تم اعتماد طلب {CitizenName} للانضمام كمصلح مع عدم وجود رسوم.",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.AcceptJoiningRequest.NoFees",
                        Value = "تم قبول طلب {CitizenName} كمحكم معتمد لدى المركز السعودي للتحكيم العقاري مع عدم وجود رسوم.",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.JoiningRequest.Updated",
                        Value = "تم تعديل طلب بالانضمام كمحكم للمحكم {CitizenName}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.JoiningRequest.Updated",
                        Value = "تم تعديل طلب بالانضمام كمصلح للمصلح {CitizenName}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.UpdateJoiningRequest.WithFees",
                        Value = "تنويه بانه تم إعادة طلب {CitizenName}  للانضمام كمحكم مع وجود رسوم للتعديل لوجود ملاحظات لمزيد من المعلومات زيارة منصة المركز",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.UpdateJoiningRequest.WithFees",
                        Value = "تم إعادة طلب انضمام {CitizenName} للتعديل لمزيد من المعلومات زيارة منصة المركز",

                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.UpdateJoiningRequest.NoFees",
                        Value = "تم إعادة طلب انضمام {CitizenName} للتعديل لمزيد من المعلومات زيارة منصة المركز",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.UpdateJoiningRequest.NoFees",
                        Value = "تنويه بانه تم إعادة طلب {CitizenName} للانضمام كمحكم مع عدم وجود رسوم للتعديل لوجود ملاحظات لمزيد من المعلومات زيارة منصة المركز",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.RejectJoiningRequest",
                        Value = "نعتذر {CitizenName} لعدم قبولكم كمصلح معتمد لدى المركز السعودي للتحكيم العقاري ولمزيد من التفاصيل زيارة منصة المركز.",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.RejectJoiningRequest",
                        Value = "نعتذر لعدم قبولكم {CitizenName} كمحكم معتمد لدى المركز السعودي للتحكيم العقاري ولمزيد من التفاصيل زيارة منصة المركز.",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.Activate",
                        Value = "تم تفعيل حساب {CitizenName} كمصلح معتمد",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.Activate",
                        Value = "تم تفعيل حساب {CitizenName} كمحكم معتمد لدى المركز السعودي للتحكيم العقاري",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reformer.DeActivate",
                        Value = "تم إيقاف حسابكم {CitizenName} لدى المركز السعودي للتحكيم العقاري، ولمزيد من التفاصيل زيارة منصة المركز.  كمصلح معتمد",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrator.DeActivate",
                        Value = "تم إيقاف حسابكم {CitizenName} كمحكم معتمد لدى المركز السعودي للتحكيم العقاري",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "Arbitration.Arbitraor.Assigned",
                        Value = "تم إضافة دعوى جديدة رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "Arbitration.ArbitraorChairman.Assigned",
                        Value = "تم قبول طلبك كرئيس هيئة تحكيم على الدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "Arbitration.ArbitraorAssigned.SendToChairman",
                        Value = "عزيزي {ArbitratorCahirman} \r\nنود إبالغكم بأن الدعوى رقم {LitigationCode} قد تم إحالتها إلي مرحلة التحكيم وتم استلامها من المحكم {ArbitratorName}\r\n",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Reconciliator.AcceptTask",
                        Value = "تم قبول الدعوى رقم {LitigationCode} من قبل المصلح {ReconciliatorName} والبدء بإجراءات الصلح",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "Arbitration.Arbitraor.AcceptTask",
                        Value = "تم قبول طلب المهمة لدعوى رقم {LitigationCode} من قبل المحكم {ArbitratorName} والبدء بإجراءات التحكيم",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Reconciliator.RejectTask",
                        Value = "تم رفض المصلح/ {ReconciliatorName} للدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "Arbitration.Arbitraor.RejectTask",
                        Value = "تم رفض المحكم/ {ArbitratorName} للدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.ReSendTask",
                        Value = "تم إرسال مهمة العمل كمصلح بالدعوى رقم {LitigationCode} للمصلح {ReconciliatorName}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitraorRecusal.AddRequest",
                        Value = "تم تقديم طلب رد المحكم {ArbitratorName} للدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitraorRecusal.AddRequest.Approve",
                        Value = "تم قبول طلب رد محكم {ArbitratorName} للدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitraorRecusal.AddRequest.Reject",
                        Value = "تم رفض طلب رد محكم {ArbitratorName} للدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitraorRecusal.InformArbitrator.Reject",
                        Value = "تم إلغاء مهمتك كمحكم على الدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitraorRecusal.InformArbitratorChairman.Rejected",
                        Value = "تم إلغاء مهمتك كرئيس هيئة تحكيم على الدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitratorChairman.NewTask",
                        Value = "يوجد طلب مهمة جديد كرئيس هيئة تحكيم على الدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitratorChairman.NewTask.Admin",
                        Value = "تم تحديد رئيس هيئة التحكيم للدعوة رقم {LitigationCode} وإرسال طلب المهمة ",
                    },new SysSetting
                    {

                       Id = Guid.NewGuid(),
                        Key = "ArbitratorsChairmanNominationList.MakeDecision",
                        Value = "تم الإنتهاء من تحديد اولوية الاختيار لرئيس هيئة التحكيم من قبل المحكمين  في الدعوي المسجلة  برقم {LitigationCode} ولابد من اتخاذ القرار ",
                    }
                    // 
                     ,new SysSetting
                    {

                       Id = Guid.NewGuid(),
                        Key = "ArbitratorsChairmanNominationList.TaskSubmitedToArbitrator",
                        Value = "يوجد طلب مهمه جديد كرئيس هية التحكيم لدعوي رقم {LitigationCode}",
                    }
                    ,new SysSetting
                    {

                       Id = Guid.NewGuid(),
                        Key = "ArbitratorsChairmanNominationList.TaskSubmited",
                        Value = "تم الإنتهاء من تحديد اولوية الاختيار لرئيس هيئة التحكيم من قبل المحكمين و ارسال طلب المهمة للمحكم {ArbitratorName} في الدعوي المسجلة برقم {LitigationCode} ",
                    }
                    ,new SysSetting
                    {

                       Id = Guid.NewGuid(),
                        Key ="ArbitratorsChairmanNominationList.Added" ,
                        Value = "تم إصدار قائمة المحكمين للدعوى رقم {LitigationCode}، يرجى ترتيب المحكمين بحسب رغبتكم",
                    }
                    ,new SysSetting
                    {


                       Id = Guid.NewGuid(),
                        Key = "ArbitratorsNominationList.AddedForAdmin",
                        Value = "تم إضافة قائمة تعيين هيئة التحكيم في الدعوي المسجلة برقم {LitigationCode} ",
                    }
                    ,new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitratorsNominationList.Added",
                        Value = " تم أحالة طلبكم للتحكيم في الدعوة {LitigationCode}, لاختيار المحكم يمكنك الدخول الي حسابك في منصة التحكيم العقاري واختيار المحكم المرشح,علما بأنه سيتم انتهاء صلاحية ترشيح محكم في خلال {ExpirationDate}",
                    },new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitratorsNominationList.AssignedAndSent",
                        Value = "تم تعيين هيئة التحكيم وإرسال طلبات المهام لدعوي رقم {LitigationCode}",
                    },

                    new SysSetting
                    {
                       Id = Guid.NewGuid(),
                        Key = "ArbitratorsChairmanNominationListReply.Added",
                        Value = "تم الإنتهاء من تحديد اولوية الاختيار لرئيس هيئة التحكيم من قبل المحكمين لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitration.ExplanationReport.Added",
                        Value ="تم تقديم طلب تفسير حكم التحكيم للدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitration.ExplanationReport.RepliedOn",
                        Value = "تم الرد على طلب تفسير حكم التحكيم لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitration.ExtendRequest.Added",
                        Value = "عزيزي {ArbitratorCahirman}\r\n نود إبلاغكم بانه قام المحكم {ArbitratorName}\r\n بطلب تمديد مدة التحكيم علي الدعوى رقم {LitigationCode}\r\n",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitration.ExtendRequest.Accepted",
                        Value = "تم الموافقة على طلب التمديد للدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitration.ExtendRequest.Rejected",
                        Value = "تم رفض طلب التمديد لدعوي رقم {LitigationCode} ولمزيد من التفاصيل زيارة منصة المركز",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrators.Count.Added",
                        Value = "تم الإنتهاء من تحديد عدد المحكمين النهائي لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrators.Count.Added.AfterAgreement",
                        Value = "تم الإنتهاء من تحديد عدد المحكمين النهائي لاتفاق كلا المدعى والمدعى عليه على عدد المحكمين لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Arbitrators.Count.FromBothSides",
                        Value = "تم الإنتهاء من تحديد عدد المحكمين من قبل المدعى لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ArbitratorSelectionDuration.Added",
                        Value = "تم تحديد مدة أختيار المحكمين لدعوي رقم {LitigationCode} , يرجي تحديد عدد هيئة التحكيم للدعوى",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ArbitratorSelectionDuration.Added.Admin",
                        Value = "تم تحديد مدة أختيار المحكمين لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ArbitrationReport.Added.FirstParty",
                        Value = "عزيزي {FirstPartyName}\r\n نود إبلاغكم بانه تم إصدار حكم التحكيم علي الدعوى رقم {LitigationCode}\r\n برجاء الدخول علي موقع المنصه من خلال الرابط {PortalUrl}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ArbitrationReport.Added.SecondParty",
                        Value = "عزيزي {SecondPartyName}\r\n نود إبلاغكم بانه تم إصدار حكم التحكيم علي الدعوى رقم {LitigationCode}\r\n برجاء الدخول علي موقع المنصه من خلال الرابط {PortalUrl}",
                    },

                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ArbitrationService.JudgmentAdded",
                        Value = "تم اضافةالحكم في الدعوي المسجلة برقم {LitigationCode} برجاء الدخول علي موقع المنصه من خلال الرابط {PortalUrl}",
                    }
                    ,new SysSetting
                    {

                        Id = Guid.NewGuid(),
                        Key = "ArbitrationSession.Added",
                        Value = "تم اضافة موعد لجلسة الحكم في الدعوي المسجلة برقم {LitigationCode} رابط الجلسه {SessionLink} ",
                    },
                   new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ExcuteWorkflow",
                        Value = "تم تقديم دعوى جديدة رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ExcuteWorkflow.DelegatorFirstParty",
                        Value = "تم تقديم دعوى جديدة رقم {LitigationCode} بالنيابة عنك",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "DelegationRequest.Accepted",
                        Value = "تم قبول طلب التفويض المقدم لدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "DelegationRequest.Rejected",
                        Value = "تم رفض طلب التفويض المقدم لدعوى رقم {LitigationCode} بسبب {RejectReason}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "DelegationRequest.Added",
                        Value = "تم تقديم طلب تفويض جديد لدعوى رقم {LitigationCode} و هو حاليا قيد المراجعة",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.ModificationRequest",
                        Value = "تم أضافة طلب تعديل علي الدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ProcessWorkflow.Accepted",
                        Value = "تم قبول طلب الدعوي رقم {LitigationCode} \r\nبرجاء دفع رسوم قبول الطلب في مدة اقصاها {ExpirationDays} يوم \r\n",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ProcessWorkflow.Reject",
                        Value = "تم رفض الطلب للدعوى رقم {LitigationCode} بسبب {RejectionReason} ولمزيد من المعلومات فضلا زيارة منصة المركز السعودي للتحكيم العقاري",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ProcessWorkflow.SentBackForEdit",
                        Value = "تم إعادة الطلب لوجود بعض الملاحظات لدعوى رقم {LitigationCode} علما بأنه سيتم انتهاء صلاحية التعديل في خلال {ExpirationDate}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ProcessWorkflow.Edited",
                        Value = "تم تعديل الدعوى وإرسالها مرة أخرى رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Cancelled",
                        Value = "تم إلغاء المهمة (دعوى صلح)  رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.ReAssigned",
                        Value = "تم إرسال مهمة مصلح لمصلح أخر بأسم {ReformerName} لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Assign",
                        Value = "تم ارسال طلب مهمة كمصلح على الدعوى رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Creation.Reject",
                        Value = "تم رفض الطلب لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Creation.Resent",
                        Value = "تم إعادة الطلب للتعديل للدعوى رقم {LitigationCode} ويجب تعديل الطلب خلال {ExpirationDate} يوم لتفادي اغلاق الدعوى",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Creation.Accept",
                        Value = "تم قبول واستلام الطلب لدعوي رقم {LitigationCode}",
                    }
                    ,new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Creation.Accept.Defendant",
                        Value = "تم تقديم دعوى رقم {LitigationCode} لدى المركز السعودي للتحكيم العقاري والمقامة ضدكم، وللإطلاع على تفاصيل الدعوى الدخول عبر الرابط التالي: {PortalUrl} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Decision.WaiverOfRight",
                        Value = "تم إصدار محضر صلح للدعوى رقم {LitigationCode} يرجى الرد على محضر الصلح",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Decision.ReconciliationWasMade",
                        Value = "تم إصدار محضر صلح للدعوى رقم {LitigationCode} يرجى الرد على محضر الصلح",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Decision.ReconciliationAccepted",
                        Value = "تم اعتماد محضر الصلح للدعوى رقم {LitigationCode} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Decision.WaiverOfTheLitigation",
                        Value = "تم إصدار محضر صلح للدعوى رقم {LitigationCode} يرجى الرد على محضر الصلح",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Decision.ReconciliationWasNotMadeAndMoveToArbitration",
                        Value = "تم إصدار محضر عدم صلح للدعوى رقم {LitigationCode} وإحالة الدعوى للتحكيم",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Decision.ReconciliationWasNotMade",
                        Value = "تم إصدار محضر عدم صلح للدعوى رقم {LitigationCode} ",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.ExtendRequest.Added",
                        Value = "تم اضافة طلب مد مدة الصلح لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.ExtendRequest.Accepted",
                        Value = "تم قبول طلب مد مدة الصلح لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.ExtendRequest.Rejected",
                        Value = "تم رفض طلب مد مدة الصلح لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ReconciliationSession.Set",
                        Value = "تم تحديد موعد لجلسة الصلح\r\nرقم الدعوى: {LitigationCode} برجاء التواجد فى {SessionDatetime}, رابط الجلسه {SessionLink}",

                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "ReconciliationSession.Reset",
                        Value = "تم أعادة تعيين جلسة الصلح\r\nترقم الدعوى: {LitigationCode} برجاء التواجد فى {SessionDatetime}, رابط الجلسه {SessionLink}",

                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Workflow.Condition",
                        Value = "يوجد طلب مشارطة الدعوى رقم {LitigationCode}",
                    }
                    ,
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Workflow.Condition.Review",
                        Value = "تم ارسال طلب مشارطة التحكيم للدعوى رقم {LitigationCode}",
                    }, new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.NewRequest",
                        Value = "يوجد طلب مهمة جديدة كمصلح على الدعوى رقم {LitigationCode}",

                    }, new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.NewRequest.ManualDistripution",
                        Value = "يوجد دعوي جديده رقم {LitigationCode} , برجاء تعيين مصلح عليها",

                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "Reconciliation.Request.Added",
                        Value = "تم إضافة محضر الصلح لدعوى رقم {LitigationCode} برجاء الدخول علي موقع المنصه من خلال الرابط {PortalUrl}"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Accept.Agreement",
                        Value = "تم الموافقة على طلب المشارطة على الدعوى رقم {LitigationCode}"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Reject.Agreement",
                        Value = "تم رفض طلب المشارطة على الدعوى رقم {LitigationCode}"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequestCorrespondenceAdded",
                        Value = "يوجد رسالة واردة في منصة المركز رقم {CorrespondenceCode} علي الدعوي رقم {LitigationCode}"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequestCorrespondenceReplyAdded",
                        Value = "تم اضافة رد علي المراسلة رقم {CorrespondenceCode} علي الدعوي رقم {LitigationCode}"
                    }
                    ,new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.Decision.ReconciliationOnTradyPlatform",
                        Value = "تم إصدار محضر صلح في منصة تراضى لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.CollectionOfLitigationFeesRequestIsBeingSent",
                        Value = "تم إصدار فاتورة الرسوم الإدارية للدعوى رقم {LitigationCode} عند سداد الرسوم فضلا تأكيد السداد عبر منصة المركز",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.LitigationFeesHaveBeenCollected",
                        Value = "تم دفع رسوم الدعوي لدعوي رقم {LitigationCode}",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.CollectionOfReconciliationFeesRequestIsBeingSent",
                        Value = "تم إصدار رسوم المصالحة للدعوى رقم {LitigationCode}، عند سداد الرسوم فضلا تأكيد السداد عبر منصة المركز",
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "LitigationRequest.ReconciliationFeesHaveBeenCollected",
                        Value = "تم دفع رسوم الصلح لدعوي رقم {LitigationCode}",
                    },
	                #endregion
                  

                    #region Email Setting Configuration
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "EmailConfiguration_URL",
                            Value = "https://api-dev.rega.gov.sa/v1/rega/notificationservice/send-email"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "EmailConfiguration_SendEmail",
                            Value = "true"

                        },
                         new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "EmailConfiguration_Host",
                            Value = "smtp.gmail.com",

                        },
                          new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "EmailConfiguration_Port",
                            Value = "587",

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "EmailConfiguration_FromEmail",
                            Value = "asgatech602@gmail.com",

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "EmailConfiguration_DisplayNameFromEmail",
                            Value = "Off plan Report",

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "EmailConfiguration_Passowrd",
                            Value = "rftl tfex qlrf zsay",

                        },
                            new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "EmailConfiguration_UseDefaultCredentials",
                            Value = "false",

                        },
                        //    new SysSetting
                        //{
                        //    Id = Guid.NewGuid(),
                        //    Key = "EmailConfirmation_URL",
                        //    Value = "https://wafiservices.rega.gov.sa/wafi/confirmemail?token={0}&email={1}",

                        //},
                    #endregion

                    #region SLA
                    #region Review 
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.AgreementPeriodFinishedWarning",
                            Value = "انتهي وقت المشارطه لدعوي رقم {LitigationCode} برجاء استكمال الدعوي الي مرحلة الصلح"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestReconciliationFeesReminder",
                            Value = "يرجى سرعة سداد رسوم المصالحة للدعوى رقم {LitigationCode} تفادياً لإغلاق الدعوى"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitrationFeesReminder",
                            Value = "يرجى سرعة سداد رسوم اتعاب هيئة التحكيم للدعوى رقم {LitigationCode} تفادياً لإغلاق الدعوى"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReminderForReviewer",
                            Value = "72"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestReconciliationFeesNotPaidReminderFirstPeriod",
                            Value = "72"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestReconciliationFeesNotPaidReminderSecondPeriod",
                            Value = "144"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReminderForReviewAdmin.ReAssign",
                            Value = "فضلا إعادة تعيين مدقق للدعوى رقم {LitigationCode} لانتهاء مدة التدقيق"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReminderForApplicant.EditLitigationPeriod",
                            Value = "24"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestFeesReminder",
                            Value = "يرجى سرعة سداد الرسوم الإدارية للدعوى رقم {LitigationCode} تفادياً لإغلاق الدعوى"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReminderForReviewerManager",
                            Value = "144"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.FirstEditReminderForRequestApplicant",
                            Value = "24"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.SecondEditReminderForForRequestApplicant",
                            Value = "72"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.FirstAgreementRequestForRequest",
                            Value = "24"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.SecondAgreementRequestForForRequest",
                            Value = "72"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestFeesNotPaidReminderFirstPeriod",
                            Value = "72"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestFeesNotPaidReminderSecondPeriod",
                            Value = "124"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitrationFeesNotPaidReminderFirstPeriod",
                            Value = "72"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitrationFeesNotPaidReminderSecondPeriod",
                            Value = "144"

                        },new SysSetting
                        {
                        Id = Guid.NewGuid(),
                        Key = "Review.Cancelled",
                        Value = "تم إلغاء المهمة  رقم {LitigationCode}",
                        },new SysSetting
                        {
                        Id = Guid.NewGuid(),
                        Key = "Review.ReAssigned",
                        Value = "تم إرسال مهمة مدقق الى مدقق اّخر المهمة  رقم {LitigationCode}",
                        },
                    #endregion
                    #region Reconciliation
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestExtendRequestReplyPeriod",
                            Value = "24"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReformerReminderForReconciliationFirstPeriod",
                            Value = "72"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReformerReminderForReconciliationSecondPeriod",
                            Value = "144"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestReconciliationReportResponsePeriod",
                            Value = "24"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestReconciliationReportDecisionPeriod",
                            Value = "72"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestReformerAssigningRequestResponsePeriod",
                            Value = "24"

                        },
                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestReformerAssigningRequestDecisionPeriod",
                            Value = "72"

                        },
                    #endregion
                    #region Arbitrator Registeration
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ArbitratorRegisterationFees.A",
                            Value = "300"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ArbitratorRegisterationFees.B",
                            Value = "200"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ArbitratorRegisterationFees.C",
                            Value = "100"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorRegisterationFeesPeriod",
                            Value = "72"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorRegisterationFeesFirstNotificationPeriod",
                            Value = "48"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorRegisterationFeesNotification",
                            Value = "برجاء دفع رسوم التسجيل كمحكم قبل انتهاء المدة المحددة لذلك"

                        },
	                #endregion
                    #region Arbitrator Membership
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ArbitratorMembershipFees.A",
                            Value = "300"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ArbitratorMembershipFees.B",
                            Value = "200"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ArbitratorMembershipFees.C",
                            Value = "100"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorMembershipFeesPeriod",
                            Value = "72"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorMembershipFeesFirstNotificationPeriod",
                            Value = "48"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorMembershipFeesNotification",
                            Value = "برجاء دفع رسوم العضوية كمحكم قبل انتهاء المدة المحددة لذلك"

                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorMembershipRenewalFeesNotification",
                            Value = "برجاء دفع رسوم تجديد العضوية كمحكم قبل انتهاء المدة المحددة لذلك"

                        },
	                #endregion
                    #region Sla NotificationMessages
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.PayLitigationFees.Applicant",
                            Value = "تم الغاء الدعوي رقم {LitigationCode} لعدم دفع الرسوم  "
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.PayLitigationReconciliationFees.Applicant",
                            Value = "تم الغاء الدعوي رقم {LitigationCode} لعدم دفع رسوم الصلح  "
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.PayLitigationArbitrationFees.Applicant",
                            Value = "تم الغاء الدعوي رقم {LitigationCode} لعدم دفع رسوم التحكيم  "
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.FinishLitigation.ReviewerEmployee",
                            Value = "يرجى اتمام مراجعة الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.WithdrewLitigation.OldReviewerEmployee",
                            Value = "تم سحب دعوي رقم {LitigationCode} لتخطى مدة التدقيق الفصوى"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.AssignLitigation.NewReviewerEmployee",
                            Value = "تم ارسال دعوى جديدة رقم {LitigationCode} للتدقيق"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.AssignLitigation.ReviewerAdmin",
                            Value = "تم ارسال الدعوى رقم {LitigationCode} إلى مدقق جديد لتخطى المدقق السابق مدة التدقيق القصوى"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReviewDelay.ReviewerAdmin",
                            Value = "يوجد تأخير تجاوز المده المسموح بها للتدقيق على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.FinishNotes",
                            Value = "يرجى سرعة معالجة كافة الملاحظات على الدعوى رقم {LitigationCode} خلال {ExpirationDate} ايام والا ستصبح ملغاة"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationCancelled",
                            Value = "تم إلفاء الدعوى رقم {LitigationCode} لعدم الرد على الملاحظات المرسلة"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.FinishAgreementRequest",
                            Value = "يرجى سرعة الرد على طلب المشارطة المرسل على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.AgreementCancelled",
                            Value = "تم إلفاء الدعوى رقم {LitigationCode} لعدم الرد على طلب المشارطة"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.FinishReconciliation",
                            Value = "يرجى سرعة إتمام إجراءات المصالحة قبل انتهاء الوقت على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.FinishReconciliation.WarnReconciliationAdmin",
                            Value = "يرجى العلم أن المصلح {ReformerName} تجاوز الفترة المسموح بها للصلح على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReconciliationReport.PleaseReply",
                            Value = "يرجى سرعة الرد على محضر الصلح الصادر على الدعوى رقم {LitigationCode} برجاء الدخول علي موقع المنصه من خلال الرابط {PortalUrl}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReconciliationReport.WarnReformer",
                            Value = "يرجى العلم أن الرد على محضر الصلح الصادر على الدعوى رقم {LitigationCode} قد تجاوز مهلته القصوى"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReconciliationTask.PleaseReply",
                            Value = "يرجى سرعة الرد على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReconciliationTaskCancelled",
                            Value = "تم إلغاء طلب مهمة الصلح على الدعوى رقم {LitigationCode} لتجاوز الرد على المهمة مهلته القصوى "
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReconciliationTask.New",
                            Value = "يوجد طلب مهمة صلح جديد على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ReconciliationTask.ReAssigned",
                            Value = "يرجى العلم أن الرد على مهمة الصلح للمصلح {PreviousReformer} على الدعوى رقم {LitigationCode} قد تجاوز مهلته القصوى وتم إلغاؤها وارسال مهمة جديدة للمصلح {CurrentReformer}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsCount.FirstWarning",
                            Value = "يرجى سرعة اتمام عملية إختيار عدد المحكمين على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsCount.SecondWarning",
                            Value = "تذكير يرجى سرعة اتمام عملية إختيار عدد المحكمين على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsCount.FinalWarning.Plantiff",
                            Value = "لم يقم المدعى بتحديد عدد المحكمين على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsCount.FinalWarning.Defendant",
                            Value = "لم يقم المدعى عليه بتحديد عدد المحكمين على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsCount.FinalWarning.Both",
                            Value = "لم يقم المدعى والمدعى عليه بتحديد عدد المحكمين على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsSelect.FirstWarning",
                            Value = "يرجى سرعة اتمام عملية إختيار هيئة التحكيم على الدعوى رقم {LitigationCode}"
                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsSelect.FinalWarning.Plantiff",
                            Value = "لم يقم المدعى بإنهاء تعيين هيئة المحكمين على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsSelect.FinalWarning.Defendant",
                            Value = "لم يقم المدعى عليه بإنهاء تعيين هيئة المحكمين على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsSelect.FinalWarning.Both",
                            Value = "لم يقم المدعى والمدعى عليه بإنهاء تعيين هيئة المحكمين على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsChairmanSelect.FirstWarning",
                            Value = "يرجى سرعة اتمام عملية إختيار رئيس هيئة التحكيم على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorsChairmanSelect.FinalWarning.Admin",
                            Value = "لم يتم تعيين رئيس هيئة التحكيم على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorTaskRequest.FirstWarning",
                            Value = "يرجى سرعة الرد على طلب مهمة التحكيم على الدعوى رقم {LitigationCode}"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorTaskRequest.FirstWarning.Admin",
                            Value = "لم يقم المحكم {Arbitrator} بقبول طلب المهمه"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorTaskRequest.Cancelled",
                            Value = "تم إلغاء طلب مهمة التحكيم كمحكم على الدعوى رقم {LitigationCode} لتجاوز الرد على المهمة مهلته القصوى"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorTaskRequest.ReAssign",
                            Value = "تم إرسال طلب مهمة تحكيم جديدة كمحكم على الدعوى رقم {LitigationCode} "
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorChairmanTaskRequest.Cancelled",
                            Value = "تم إلغاء طلب مهمة التحكيم كرئيس هيئة تحكيم على الدعوى رقم {LitigationCode} لتجاوز الرد على المهمة مهلته القصوى"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.ArbitratorChairmanTaskRequest.ReAssign",
                            Value = "تم إرسال طلب مهمة تحكيم جديدة كرئيس هيئة على الدعوى رقم {LitigationCode} "
                        },
	                #endregion
#region Arbitration
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SLA.AddLitigationRequestArbitratorNominationListPeriod",
                        Value = "48"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SLA.AddLitigationRequestArbitratorNominationListWarning",
                        Value = "برجاء سرعة اضافة قائمة المحكمين المرشحين لدعوي رقم {LitigationRequest}"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SLA.AddLitigationRequestArbitrationSessionPeriod",
                        Value = "24"
                    },new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = "SLA.AddLitigationRequestArbitrationSessionWarning",
                        Value = "يرجى سرعة إتمام إجراءات التحكيم قبل انتهاء الوقت لدعوي رقم {LitigationRequest}"
                    },
	#endregion
                    #endregion

                    new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Reconciliation.ReformerMaxSessionsCountPerDay",
                            Value = "3"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Arbitration.ArbitratorMaxSessionsCountPerDay",
                            Value = "3"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Arbitration.LitigationRequestArbitratorRecusalRequestPeriod",
                            Value = "24"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Arbitration.LitigationRequestArbitrationReportExplanationRequestPeriod",
                            Value = "24"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorCountSelectionResponsePeriod",
                            Value = "24"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorCountSelectionDecisionPeriod",
                            Value = "72"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorNominationListResponsePeriod",
                            Value = "24"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorNominationListDecisionPeriod",
                            Value = "72"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorChairmanNominationListResponseFirstPeriod",
                            Value = "24"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorChairmanNominationListDecisionSecondPeriod",
                            Value = "72"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorAssigningRequestResponseFirstPeriod",
                            Value = "24"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorAssigningRequestResponseSecondPeriod",
                            Value = "72"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitratorAssigningRequestDecisionPeriod",
                            Value = "144"

                        },
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "SLA.LitigationRequestArbitrationReportExplanationRequestResponsePeriod",
                            Value = "72"
                        },

                        #region Zoom
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Zoom.ClientID",
                            Value = "sqlMYoQ320xzHiEvPYCA"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Zoom.ClientSecret",
                            Value = "wBufFSQdMcNG6rb5l8mYn1rGLCtP0BY4"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Zoom.AccountID",
                            Value = "2u0TjNu3QsmBhBa1yYOMTw"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Zoom.MeetingDuration",
                            Value = "60"
                        },

	                    #endregion

                        #region Configurable Settings
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ArbitratorsCountSelectDuration",
                            Value = "24"
                        },
	                    #endregion

                    #region Invoice 
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = SysSettingKeyConstants.Invoice_Taxes,
                        Value = "0"

                    },
                    new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = SysSettingKeyConstants.Invoice_ExpirationDays,
                        Value = "5"

                    },
                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = SysSettingKeyConstants.Invoice_CreatedInvoiceMessage,
                        Value = "تم إصدار رسوم اتعاب هيئة التحكيم برقم فاتورة {InvoiceCode} للدعوى رقم {LitigationCode}، عند سداد الرسوم فضلا تأكيد السداد عبر منصة المركز." 

                    },
                     new SysSetting
                    {
                        Id = Guid.NewGuid(),
                        Key = SysSettingKeyConstants.Invoice_ArbitrationInvoicePaidMessage,
                        Value = "تم سداد رسوم التحكيم للدعوي رقم {LitigationCode}"

                    },
	                #endregion

                        #region Fees
                        new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ReconciliationFees",
                            Value = "50"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ReformerJoiningRequestFees",
                            Value = "20"
                        },new SysSetting
                        {
                            Id = Guid.NewGuid(),
                            Key = "Configurable.ArbitratorRegistrationFees",
                            Value = "30"
                        },
                        #endregion
                });


            context.SaveChanges();
        }

    }
}
