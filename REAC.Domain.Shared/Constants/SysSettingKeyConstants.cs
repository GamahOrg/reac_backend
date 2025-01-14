namespace REAC.Domain.Shared.Constants
{
    public class SysSettingKeyConstants
    {
        public const string SecurityApiUrl = "SecurityApiUrl"; // http://localhost:7144/

        #region Integration Keys

        public const string MC_Key = "MC";
        public const string MOJ_Key = "MOJ";
        public const string GEE_Key = "GEE";
        public const string Nafath_Key = "Nafath";
        public const string NafathCheck_Key = "NafathCheck";

        #endregion

        #region Login GEE Integration keys

        public const string GEE_LoginURL = "GEE_LoginURL";
        public const string GEE_grant_type = "GEE_grant_type";
        public const string GEE_client_secret = "GEE_client_secret";
        public const string GEE_client_id = "GEE_client_id";

        #endregion

        #region MOJ Integration keys

        public const string MOJ_URL = "MOJ_URL";
        public const int MOJControlId = 82007;

        #endregion

        #region MC Integration keys

        public const string MC_GetCRInformationByCRNumber_URL = "MC_GetCRInformationByCRNumber_URL";
        public const string MC_GetCRInformationByCROwners_URL = "MC_GetCRInformationByCROwners_URL";

        #endregion

        #region SMS Integration keys

        public const string Disable_SMS_Integration = "Disable_SMS_Integration";
        public const string SMS_URL = "SMS_URL";
        public const string SMS_Notification_Type = "SMS_Notification_Type";
        public const string Litigation_Request_Max_Session_ReScheduleTime = "Litigation_Request_Max_Session_ReScheduleTime";


        #endregion

        #region Nafath Integration keys

        public const string Nafath_URL = "Nafath_URL";
        public const string Nafath_Action = "Nafath_Action";
        public const string Nafath_Service = "Nafath_Service";

        #endregion

        #region Nafath Check keys

        public const string NafathCheck_Action = "NafathCheck_Action";
        public const string NafathCheck_URL = "NafathCheck_URL";

        #endregion

        #region Get User Info

        public const string NafathGetUserInfo_URL = "NafathGetUserInfo_URL";
        public const string GEENafathEnableNafathToken_Key = "GEENafathCheckEnableNafathToken";

        #endregion

        #region Get User Address Info

        public const string NafathGetUserAddressInfo_URL = "NafathGetUserAddressInfo_URL";

        #endregion

        #region Wafi
        public const string Wafi_URL = "Wafi_URL";

        #endregion

        #region Ejar
        public const string Ejar_ContractDetails = "Ejar_ContractDetails";
        public const string Ejar_ContractFinancialDetails = "Ejar_ContractFinancialDetails";

        #endregion

        #region Nafath Response Status keys

        public const string NafathStatus_COMPLETED = "COMPLETED";
        public const string NafathStatus_WAITING = "WAITING";
        public const string NafathStatus_REJECTED = "REJECTED";
        public const string NafathStatus_EXPIRED = "EXPIRED";

        #endregion






        public const string Arbitration_ExpirationDateHours = "Arbitration.ExpirationDateHours";
        public const string Arbitration_Period = "Arbitration.Period";
        public const string Reconciliation_Period = "Reconciliation.Period";
        public const string Review_Period = "Review.Period";
        public const string Reconciliation_Request_Study_Period = "Reconciliation.Request.Study.Period";
        public const string Reconciliation_Request_Process_Notes_Period = "Reconciliation.Request.Process.Notes.Period";
        public const string Reconciliation_Reformer_Approve_Mission_Period = "Reconciliation.Reformer.Approve.Mission.Period";
        public const string Reconciliation_LitigationParties_Response_Period = "Reconciliation.LitigationParties.Response.Period";
        public const string Reconciliation_Report_Creation = "Reconciliation.Report.Creation";


        #region systemNotificationMessages

        public const string Arbitrator_UpdateJoiningRequest_ToPayFees = "Arbitrator.UpdateJoiningRequest.ToPayFees";
        public const string Employee_Arbitrator_UpdateJoiningRequest_ToPayFees = "Employee.Arbitrator.UpdateJoiningRequest.ToPayFees";

        public const string Reformer_UpdateJoiningRequest_ToPayFees = "Reformer.UpdateJoiningRequest.ToPayFees";
        public const string Employee_Reformer_UpdateJoiningRequest_ToPayFees = "Employee.Reformer.UpdateJoiningRequest.ToPayFees";

        public const string Arbitrator_UpdateJoiningRequest_PaidFees = "Arbitrator.UpdateJoiningRequest.PaidFees";
        public const string Employee_Arbitrator_UpdateJoiningRequest_PaidFees = "Employee.Arbitrator.UpdateJoiningRequest.PaidFees";

        public const string Reformer_UpdateJoiningRequest_PaidFees = "Reformer.UpdateJoiningRequest.PaidFees";
        public const string Employee_Reformer_UpdateJoiningRequest_PaidFees = "Employee.Reformer.UpdateJoiningRequest.PaidFees";

        public const string Citizen_AddBankAccount = "Citizen.AddBankAccount";
        public const string Citizen_EditBankAccount = "Citizen.EditBankAccount";
        public const string Citizen_AddAgent = "Citizen.AddAgent";
        public const string Delegator_AddAgent = "Delegator.AddAgent";
        public const string Citizen_AddLawyer = "Citizen.AddLawyer";
        public const string Delegator_AddLawyer = "Delegator.AddLawyer";
        public const string Citizen_DeleteLawyer = "Citizen.DeleteLawyer";
        public const string Delegator_DeleteLawyer = "Delegator.DeleteLawyer";
        public const string Reformer_JoiningRequest = "Reformer.JoiningRequest";
        public const string Arbitrator_JoiningRequest = "Arbitrator.JoiningRequest";
        public const string Reformer_AcceptJoiningRequest = "Reformer.AcceptJoiningRequest";
        public const string Arbitrator_AcceptJoiningRequest = "Arbitrator.AcceptJoiningRequest";
        public const string Arbitrator_JoiningRequest_Updated = "Arbitrator.JoiningRequest.Updated";
        public const string Reformer_JoiningRequest_Updated = "Reformer.JoiningRequest.Updated";
        public const string Arbitrator_UpdateJoiningRequest_WithFees = "Arbitrator.UpdateJoiningRequest.WithFees";
        public const string Reformer_UpdateJoiningRequest_WithFees = "Reformer.UpdateJoiningRequest.WithFees";
        public const string Arbitrator_UpdateJoiningRequest = "Arbitrator.UpdateJoiningRequest.NoFees";
        public const string Reformer_UpdateJoiningRequest = "Reformer.UpdateJoiningRequest.NoFees";
        public const string Reformer_AcceptJoiningRequest_NoFees = "Reformer.AcceptJoiningRequest.NoFees";
        public const string Arbitrator_AcceptJoiningRequest_NoFees = "Arbitrator.AcceptJoiningRequest.NoFees";
        public const string Reformer_AcceptJoiningRequest_WaitingForFees = "Reformer.AcceptJoiningRequest.WaitingForFees";
        public const string Arbitrator_AcceptJoiningRequest_WaitingForFees = "Arbitrator.AcceptJoiningRequest.WaitingForFees";
        public const string Reformer_RejectJoiningRequest = "Reformer.RejectJoiningRequest";
        public const string Arbitrator_RejectJoiningRequest = "Arbitrator.RejectJoiningRequest";
        public const string Reformer_Activate = "Reformer.Activate";
        public const string Arbitrator_Activate = "Arbitrator.Activate";
        public const string Reformer_DeActivate = "Reformer.DeActivate";
        public const string Arbitrator_DeActivate = "Arbitrator.DeActivate";
        public const string Arbitration_Arbitraor_Assigned = "Arbitration.Arbitraor.Assigned";
        public const string Arbitration_ArbitraorChairman_Assigned = "Arbitration.ArbitraorChairman.Assigned";
        public const string ArbitratorChairman_NewTask = "ArbitratorChairman.NewTask";
        public const string ArbitratorChairman_NewTask_Admin = "ArbitratorChairman.NewTask.Admin";
        public const string Arbitration_ArbitraorAssigned_SendToChairman = "Arbitration.ArbitraorAssigned.SendToChairman";
        public const string Reconciliation_Reconciliator_AcceptTask = "Reconciliation.Reconciliator.AcceptTask";
        public const string Arbitration_Arbitraor_AcceptTask = "Arbitration.Arbitraor.AcceptTask";
        public const string Reconciliation_Reconciliator_RejectTask = "Reconciliation.Reconciliator.RejectTask";
        public const string Arbitration_Arbitraor_RejectTask = "Arbitration.Arbitraor.RejectTask";
        public const string Reconciliation_ReSendTask = "Reconciliation.ReSendTask";
        public const string ArbitraorRecusal_AddRequest = "ArbitraorRecusal.AddRequest";
        public const string ArbitraorRecusal_AddRequest_Approve = "ArbitraorRecusal.AddRequest.Approve";
        public const string ArbitraorRecusal_AddRequest_Reject = "ArbitraorRecusal.AddRequest.Reject";
        public const string ArbitraorRecusal_InformArbitrator_Reject = "ArbitraorRecusal.InformArbitrator.Reject";
        public const string ArbitraorRecusal_InformArbitratorChairman_Rejected = "ArbitraorRecusal.InformArbitratorChairman.Rejected";
        public const string ArbitratorsChairmanNominationList_MakeDecision = "ArbitratorsChairmanNominationList.MakeDecision";
        public const string ArbitratorsChairmanNominationList_TaskSubmited = "ArbitratorsChairmanNominationList.TaskSubmited";
        public const string ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator = "ArbitratorsChairmanNominationList.TaskSubmitedToArbitrator";
        public const string ArbitratorsChairmanNominationList_Added = "ArbitratorsChairmanNominationList.Added";
        public const string ArbitratorsNominationList_Added = "ArbitratorsNominationList.Added";
        public const string ArbitratorsNominationList_AddedForAdmin = "ArbitratorsNominationList.AddedForAdmin";
        public const string ArbitratorsNominationList_AssignedAndSent = "ArbitratorsNominationList.AssignedAndSent";
        public const string ArbitratorsChairmanNominationListReply_Added = "ArbitratorsChairmanNominationListReply.Added";
        public const string Arbitration_ExplanationReport_Added = "Arbitration.ExplanationReport.Added";
        public const string Arbitration_ExplanationReport_RepliedOn = "Arbitration.ExplanationReport.RepliedOn";
        public const string Arbitration_ExtendRequest_Added = "Arbitration.ExtendRequest.Added";
        public const string Arbitration_ExtendRequest_Accepted = "Arbitration.ExtendRequest.Accepted";
        public const string Arbitration_ExtendRequest_Rejected = "Arbitration.ExtendRequest.Rejected";
        public const string Arbitrators_Count_Added = "Arbitrators.Count.Added";
        public const string Arbitrators_Count_Added_AfterAgreement = "Arbitrators.Count.Added.AfterAgreement";
        public const string Arbitrators_Count_FromBothSides = "Arbitrators.Count.FromBothSides";
        public const string ArbitratorSelectionDuration_Added = "ArbitratorSelectionDuration.Added";
        public const string ArbitratorSelectionDuration_Added_Admin = "ArbitratorSelectionDuration.Added.Admin";
        public const string ArbitrationReport_Added_FirstParty = "ArbitrationReport.Added.FirstParty";
        public const string ArbitrationReport_Added_SecondParty = "ArbitrationReport.Added.SecondParty";
        public const string ArbitrationSession_Added = "ArbitrationSession.Added";
        public const string ArbitrationService_JudgmentAdded = "ArbitrationService.JudgmentAdded";
        public const string ExcuteWorkflow = "ExcuteWorkflow";
        public const string ProcessWorkflow_Accepted = "ProcessWorkflow.Accepted";
        public const string ProcessWorkflow_Reject = "ProcessWorkflow.Reject";
        public const string ProcessWorkflow_SentBackForEdit = "ProcessWorkflow.SentBackForEdit";
        public const string ProcessWorkflow_Edited = "ProcessWorkflow.Edited";
        public const string Reconciliation_Cancelled = "Reconciliation.Cancelled";
        public const string Reconciliation_ReAssigned = "Reconciliation.ReAssigned";
        public const string Reconciliation_Assign = "Reconciliation.Assign";
        public const string Reconciliation_NewRequest = "Reconciliation.NewRequest";
        public const string LitigationRequest_Creation_Reject = "LitigationRequest.Creation.Reject";
        public const string LitigationRequest_Creation_Resent = "LitigationRequest.Creation.Resent";
        public const string LitigationRequest_Creation_Accept = "LitigationRequest.Creation.Accept";
        public const string LitigationRequest_Decision_WaiverOfRight = "LitigationRequest.Decision.WaiverOfRight";
        public const string LitigationRequest_Decision_ReconciliationWasMade = "LitigationRequest.Decision.ReconciliationWasMade";
        public const string LitigationRequest_Decision_WaiverOfTheLitigation = "LitigationRequest.Decision.WaiverOfTheLitigation";
        public const string ReconciliationOnTradyPlatform = "LitigationRequest.Decision.ReconciliationOnTradyPlatform";
        public const string LitigationRequest_Decision_ReconciliationWasNotMadeAndMoveToArbitration = "LitigationRequest.Decision.ReconciliationWasNotMadeAndMoveToArbitration";
        public const string LitigationRequest_Decision_ReconciliationWasNotMade = "LitigationRequest.Decision.ReconciliationWasNotMade";
        public const string LitigationRequest_Accept_Agreement = "LitigationRequest.Accept.Agreement";
        public const string LitigationRequest_Reject_Agreement = "LitigationRequest.Reject.Agreement";
        public const string Reconciliation_ExtendRequest_Added = "Reconciliation.ExtendRequest.Added";
        public const string Reconciliation_ExtendRequest_Accepted = "Reconciliation.ExtendRequest.Accepted";
        public const string Reconciliation_ExtendRequest_Rejected = "Reconciliation.ExtendRequest.Rejected";
        public const string ReconciliationSession_set = "ReconciliationSession.set";
        public const string ReconciliationSession_Reset = "ReconciliationSession.Reset";
        public const string WorkFlow_Condition = "Workflow.Condition";
        public const string WorkFlow_Condition_Review = "Workflow.Condition.Review";
        public const string Reconciliation_Request_Added = "Reconciliation.Request.Added";

        public const string LitigationRequest_CollectionOfLitigationFeesRequestIsBeingSent = "LitigationRequest.CollectionOfLitigationFeesRequestIsBeingSent";
        public const string LitigationRequest_LitigationFeesHaveBeenCollected = "LitigationRequest.LitigationFeesHaveBeenCollected";
        public const string LitigationRequest_CollectionOfReconciliationFeesRequestIsBeingSent = "LitigationRequest.CollectionOfReconciliationFeesRequestIsBeingSent";
        public const string LitigationRequest_ReconciliationFeesHaveBeenCollected = "LitigationRequest.ReconciliationFeesHaveBeenCollected";

        public const string SLA_ReminderForReviewer = "SLA.ReminderForReviewer";
        public const string SLA_ReminderForReviewerManager = "SLA.ReminderForReviewerManager";
        public const string SLA_FirstEditReminderForRequestApplicant = "SLA.FirstEditReminderForRequestApplicant";
        public const string SLA_SecondEditReminderForForRequestApplicant = "SLA.SecondEditReminderForForRequestApplicant";
        public const string SLA_FirstAgreementRequestForRequest = "SLA.FirstAgreementRequestForRequest";
        public const string SLA_SecondAgreementRequestForForRequest = "SLA.SecondAgreementRequestForForRequest";

        public const string SLA_ReformerReminderForReconciliationFirstPeriod = "SLA.ReformerReminderForReconciliationFirstPeriod";
        public const string SLA_ReformerReminderForReconciliationSecondPeriod = "SLA.ReformerReminderForReconciliationSecondPeriod";
        public const string SLA_LitigationRequestReconciliationReportResponsePeriod = "SLA.LitigationRequestReconciliationReportResponsePeriod";
        public const string SLA_LitigationRequestReconciliationReportDecisionPeriod = "SLA.LitigationRequestReconciliationReportDecisionPeriod";
        public const string SLA_LitigationRequestReformerAssigningRequestResponsePeriod = "SLA.LitigationRequestReformerAssigningRequestResponsePeriod";
        public const string SLA_LitigationRequestReformerAssigningRequestDecisionPeriod = "SLA.LitigationRequestReformerAssigningRequestDecisionPeriod";

        public const string SLA_Reconciliation_ReformerMaxSessionsCountPerDay = "SLA.Reconciliation.ReformerMaxSessionsCountPerDay";
        public const string SLA_Arbitration_ArbitratorMaxSessionsCountPerDay = "SLA.Arbitration.ArbitratorMaxSessionsCountPerDay";

        public const string SLA_Arbitration_LitigationRequestArbitratorRecusalRequestPeriod = "SLA.Arbitration.LitigationRequestArbitratorRecusalRequestPeriod";
        public const string Arbitration_LitigationRequestArbitrationReportExplanationRequestPeriod = "Arbitration.LitigationRequestArbitrationReportExplanationRequestPeriod";

        public const string SLA_LitigationRequestArbitratorCountSelectionResponsePeriod = "SLA.LitigationRequestArbitratorCountSelectionResponsePeriod";
        public const string SLA_LitigationRequestArbitratorCountSelectionDecisionPeriod = "SLA.LitigationRequestArbitratorCountSelectionDecisionPeriod";
        public const string SLA_LitigationRequestArbitratorNominationListResponsePeriod = "SLA.LitigationRequestArbitratorNominationListResponsePeriod";
        public const string SLA_LitigationRequestArbitratorNominationListDecisionPeriod = "SLA.LitigationRequestArbitratorNominationListDecisionPeriod";
        public const string SLA_LitigationRequestArbitratorChairmanNominationListResponseFirstPeriod = "SLA.LitigationRequestArbitratorChairmanNominationListResponseFirstPeriod";
        public const string SLA_LitigationRequestArbitratorChairmanNominationListDecisionSecondPeriod = "SLA.LitigationRequestArbitratorChairmanNominationListDecisionSecondPeriod";
        public const string SLA_LitigationRequestArbitratorAssigningRequestResponseFirstPeriod = "SLA.LitigationRequestArbitratorAssigningRequestResponseFirstPeriod";
        public const string SLA_LitigationRequestArbitratorAssigningRequestResponseSecondPeriod = "SLA.LitigationRequestArbitratorAssigningRequestResponseSecondPeriod";
        public const string SLA_LitigationRequestArbitratorAssigningRequestDecisionPeriod = "SLA.LitigationRequestArbitratorAssigningRequestDecisionPeriod";
        public const string SLA_LitigationRequestArbitrationReportExplanationRequestResponsePeriod = "SLA.LitigationRequestArbitrationReportExplanationRequestResponsePeriod";

        #endregion

        #region Sla Notification Messages

        public const string SLA_FinishLitigation_ReviewerEmployee = "SLA.FinishLitigation.ReviewerEmployee";
        public const string SLA_WithdrewLitigation_OldReviewerEmployee = "SLA.WithdrewLitigation.OldReviewerEmployee";
        public const string SLA_AssignLitigation_NewReviewerEmployee = "SLA.AssignLitigation.NewReviewerEmployee";
        public const string SLA_AssignLitigation_ReviewerAdmin = "SLA.AssignLitigation.ReviewerAdmin";
        public const string SLA_ReviewDelay_ReviewerAdmin = "SLA.ReviewDelay.ReviewerAdmin";
        public const string SLA_FinishNotes = "SLA.FinishNotes";
        public const string SLA_LitigationCancelled = "SLA.LitigationCancelled";
        public const string SLA_FinishAgreementRequest = "SLA.FinishAgreementRequest";
        public const string SLA_AgreementCancelled = "SLA.AgreementCancelled";
        public const string SLA_FinishReconciliation = "SLA.FinishReconciliation";
        public const string SLA_FinishReconciliation_WarnReconciliationAdmin = "SLA.FinishReconciliation.WarnReconciliationAdmin";
        public const string SLA_ReconciliationReport_PleaseReply = "SLA.ReconciliationReport.PleaseReply";
        public const string SLA_ReconciliationReport_WarnReformer = "SLA.ReconciliationReport.WarnReformer";
        public const string SLA_ReconciliationTask_PleaseReply = "SLA.ReconciliationTask.PleaseReply";
        public const string SLA_ReconciliationTaskCancelled = "SLA.ReconciliationTaskCancelled";
        public const string SLA_ReconciliationTask_New = "SLA.ReconciliationTask.New";
        public const string SLA_ReconciliationTask_ReAssigned = "SLA.ReconciliationTask.ReAssigned";

        public const string SLA_ArbitratorsCount_FirstWarning = "SLA.ArbitratorsCount.FirstWarning";
        public const string SLA_ArbitratorsCount_SecondWarning = "SLA.ArbitratorsCount.SecondWarning";
        public const string SLA_ArbitratorsCount_FinalWarning_Plantiff = "SLA.ArbitratorsCount.FinalWarning.Plantiff";
        public const string SLA_ArbitratorsCount_FinalWarning_Defendant = "SLA.ArbitratorsCount.FinalWarning.Defendant";
        public const string SLA_ArbitratorsCount_FinalWarning_Both = "SLA.ArbitratorsCount.FinalWarning.Both";
        public const string SLA_ArbitratorsSelect_FirstWarning = "SLA.ArbitratorsSelect.FirstWarning";
        public const string SLA_ArbitratorsSelect_FinalWarning_Plantiff = "SLA.ArbitratorsSelect.FinalWarning.Plantiff";
        public const string SLA_ArbitratorsSelect_FinalWarning_Defendant = "SLA.ArbitratorsSelect.FinalWarning.Defendant";
        public const string SLA_ArbitratorsSelect_FinalWarning_Both = "SLA.ArbitratorsSelect.FinalWarning.Both";
        public const string SLA_ArbitratorsChairmanSelect_FirstWarning = "SLA.ArbitratorsChairmanSelect.FirstWarning";
        public const string SLA_ArbitratorsChairmanSelect_FinalWarning_Admin = "SLA.ArbitratorsChairmanSelect.FinalWarning.Admin";
        public const string SLA_ArbitratorTaskRequest_FirstWarning = "SLA.ArbitratorTaskRequest.FirstWarning";
        public const string SLA_ArbitratorTaskRequest_Cancelled = "SLA.ArbitratorTaskRequest.Cancelled";
        public const string SLA_ArbitratorTaskRequest_ReAssign = "SLA.ArbitratorTaskRequest.ReAssign";
        public const string SLA_ArbitratorChairmanTaskRequest_Cancelled = "SLA.ArbitratorChairmanTaskRequest.Cancelled";
        public const string SLA_ArbitratorChairmanTaskRequest_ReAssign = "SLA.ArbitratorChairmanTaskRequest.ReAssign";




        #endregion


        #region Email
        public const string PrefexEmailConfiguration = "EmailConfiguration_";
        public const string EmailConfiguration_SendEmail = "EmailConfiguration_SendEmail";
        public const string EmailFromAddress = "EmailConfiguration_FromEmail";
        public const string DisplayNameFromEmailAddress = "EmailConfiguration_DisplayNameFromEmail";
        public const string EmailPassword = "EmailConfiguration_Passowrd";
        public const string UseDefaultCredentials = "EmailConfiguration_UseDefaultCredentials";
        public const string EmailHost = "EmailConfiguration_Host";
        public const string EmailPort = "EmailConfiguration_Port";
        public const string EmailConfirmationURL = "EmailConfirmation_URL";
        #endregion

        #region Zoom
        public const string Zoom_ClientID = "Zoom.ClientID";
        public const string Zoom_ClientSecret = "Zoom.ClientSecret";
        public const string Zoom_AccountID = "Zoom.AccountID";
        public const string Zoom_MeetingDuration = "Zoom.MeetingDuration";

        #endregion

        #region Correspondence
        public const string LitigationRequestCorrespondenceAdded = "LitigationRequestCorrespondenceAdded";
        public const string LitigationRequestCorrespondenceReplyAdded = "LitigationRequestCorrespondenceReplyAdded";

        #endregion

        #region Fees
        public const string Reconciliation_Fees = "Configurable.ReconciliationFees";

        #endregion
    }
}
