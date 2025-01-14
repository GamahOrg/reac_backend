
using Bpf.Application.ModuleServices;
using Core.Contracts;
using Core.Log.Application.Contracts.IAppServices;
using Core.Log.Application.Instance;
using Core.Log.Application.Manager;
using Core.Security.Contracts;
using Core.Security.Services;
using Core.Services;
using REAC.Application.AppServices;
using REAC.Application.AppServices.Integration.GEE;
using REAC.Application.AppServices.Litigation;
using REAC.Application.AppServices.Notification;
using REAC.Application.AppServices.Reconciliation;
using REAC.Application.AppServices.Reports;
using REAC.Application.AppServices.SLA;
using REAC.Application.AppServices.System;
using REAC.Application.AppServices.UserManage;
using REAC.Application.AppServices.Wpf.Business;
using REAC.Application.AppServices.Wpf.System;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.Reconciliation;
using REAC.Application.Contracts.IAppServices.Reports;
using REAC.Application.Contracts.IAppServices.SLA;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Application.Contracts.IAppServices.Wpf.Business;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.Application.Contracts.IModuleServices;
using REAC.Application.ModuleServices;
using CitizenService = REAC.Application.AppServices.CitizenService;
using ILitigationRequestReconciliationReportService = REAC.Application.Contracts.IAppServices.Litigation.ILitigationRequestReconciliationReportService;
namespace REAC.Api
{
    public static class AppServicesDi
    {
        public static void AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<DbLogger>();
            services.AddScoped<FileLogger>();
            services.AddScoped<MailLogger>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<ILitigationWorkFlowService, LitigationWorkFlowService>();
            services.AddScoped<IReconciliationService, ReconciliationService>();
            services.AddScoped<ILitigationRequestReformerAssigningRequest, LitigationRequestReformerAssigningRequestService>();
            services.AddScoped<ICitizenBankAccountService, CitizenBankAccountService>();
            services.AddScoped<ISysBankService, SysBankService>();
            services.AddScoped<IArbitrationService, ArbitrationService>();
            services.AddScoped<ILitigationRequestArbitratorAssigningRequestService, LitigationRequestArbitratorAssigningRequestService>();
            services.AddScoped<ISysMeetingLocationService, SysMeetingLocationService>();
            services.AddScoped<ISysArbitratorCountService, SysArbitratorCountService>();
            services.AddScoped<IReviewSlaService, ReviewSlaService>();
            services.AddScoped<IReconciliationSlaService, ReconciliationSlaService>();
            services.AddScoped<IArbitrationSlaService, ArbitrationSlaService>();
            services.AddScoped<ISysVacationRequestService, SysVacationRequestService>();


            services.AddScoped<ICitizenRequestService, CitizenRequestService>();
            services.AddScoped<IWorkFlowProcessingLogService, WorkFlowProcessingLogService>();
            services.AddScoped<IBpfService, BpfService>();
            services.AddScoped<IWorkFlowLogService, WorkFlowLogService>();
            services.AddScoped<IWorkFlowStepService, WorkFlowStepService>();

            //services.AddScoped < IDelegationRequestService, DelegationRequestService>();
            //services.AddScoped < IFacilityService, FacilityService>();
            //services.AddScoped < IFacilityDelegatorService, FacilityDelegatorService>();
            //services.AddScoped<ICitizenLitigationRequestLawyerAgentService, CitizenLitigationRequestLawyerAgentService>();
            services.AddScoped<ICitizenService, CitizenService>();
            services.AddScoped<IFacilityService, FacilityService>();
            services.AddScoped<ISysAreaService, SysAreaService>();
            services.AddScoped<ISysCityService, SysCityService>();
            services.AddScoped<IDelegatorService, DelegatorService>();
            services.AddScoped<ISysExemptionReductionCategoryService, SysExemptionReductionCategoryService>();
            services.AddScoped<ILitigationRequestService, LitigationRequestService>();
            services.AddScoped<ILitigationRequestAgreementRequestService, LitigationRequestAgreementRequestService>();
            services.AddScoped<ILitigationRequestReconciliationReportService, Application.AppServices.Litigation.LitigationRequestReconciliationReportService>();
            services.AddScoped<IUserProviderService, LitigationRequestContractService>();
            services.AddScoped<ILitigationRequestCorrespondenceService, LitigationRequestCorrespondenceService>();
            services.AddScoped<ILitigationRequestCorrespondenceFileService, LitigationRequestCorrespondenceFileService>();
            services.AddScoped<ILitigationRequestExtendPeriodRequestService, LitigationRequestExtendPeriodRequestService>();
            services.AddScoped<ILitigationRequestExtendPeriodRequestFileService, LitigationRequestExtendPeriodRequestFileService>();
            services.AddScoped<ILitigationRequestGroundService, Application.AppServices.Litigation.CitizenService>();
            services.AddScoped<ILitigationRequestGroundFileService, LitigationRequestGroundFileService>();
            //services.AddScoped < ILitigationRequestMeetingService, LitigationRequestMeetingService>();
            services.AddScoped<ILitigationRequestReconciliationReportItemService, LitigationRequestReconciliationReportItemService>();
            services.AddScoped<ILitigationRequestNoteService, LitigationRequestNoteService>();
            services.AddScoped<ILitigationRequestNoteFileService, LitigationRequestNoteFileService>();
            services.AddScoped<ILitigationRequestRequestService, LitigationRequestRequestService>();
            services.AddScoped<ILitigationRequestRequestFileService, LitigationRequestRequestFileService>();
            services.AddScoped<ILitigationRatingService, LitigationRatingService>();
            services.AddScoped<ISysDepartmentService, SysDepartmentService>();
            services.AddScoped<ISysEmployeeService, SysEmployeeService>();
            services.AddScoped<ISysFileManagerService, SysFileManagerService>();
            services.AddScoped<ISysGenderService, SysGenderService>();
            services.AddScoped<ISysOtherCategoryService, SysOtherCategoryService>();
            services.AddScoped<ISysIdentityTypeService, SysIdentityTypeService>();
            services.AddScoped<ISysLitigationRequestAmountService, SysLitigationRequestAmountService>();
            services.AddScoped<ISysLitigationRequestCategoryService, SysLitigationRequestCategoryService>();
            services.AddScoped<ISysLitigationRequestContractTypeService, SysLitigationRequestContractTypeService>();
            services.AddScoped<ISysLitigationRequestCorrespondenceDestinationService, SysLitigationRequestCorrespondenceDestinationService>();
            services.AddScoped<ISysLitigationRequestExtendPeriodRequestStatusService, SysLitigationRequestExtendPeriodRequestStatusService>();
            services.AddScoped<ISysLitigationRequestOwnerTypeService, SysLitigationRequestOwnerTypeService>();
            services.AddScoped<ISysLitigationRequestStatusService, SysLitigationRequestStatusService>();
            services.AddScoped<ISysRegistrationRequestService, SysRegistrationRequestService>();
            services.AddScoped<ISysSettingService, SysSettingService>();
            services.AddScoped<ISysDelegationRelationshipTypeService, SysDelegationRelationshipTypeService>();
            services.AddScoped<ISysArbitratorClassService, SysArbitratorClassService>();


            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<DbLogger>();
            services.AddScoped<FileLogger>();
            services.AddScoped<MailLogger>();
            services.AddScoped<IMailService, REAC.Application.AppServices.Email.MailService>();
            services.AddScoped<IMOJService, MOJService>();
            services.AddScoped<IGEELoginService, GEELoginService>();
            services.AddScoped<IRENTService, RENTService>();
            services.AddScoped<INafathService, NafathService>();
            services.AddScoped<IUserRegisterService, UserRegisterService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMCService, MCService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IZoomService, ZoomService>();
            services.AddSingleton<INafathTransactionService, NafathTransactionService>();
            services.AddSingleton<INafathAgreementRequestService, NafathAgreementRequestService>();
            services.AddSingleton<INafathReconciliationReportService, NafathReconciliationReportService>();

            services.AddScoped<IJwtReader, JwtReader>();
            services.AddScoped<IZoomService, ZoomService>();
            services.AddScoped<IReportService, ReportService>();

        }


    }
}