using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Domain.Shared.Constants
{
    public class SystemSettingsConfiguration
    {
        public static readonly List<string> ReconciliationSystemSettings = new List<string>
        {
            "SLA_ReformerReminderForReconciliationFirstPeriod",
            "SLA.ReformerReminderForReconciliationSecondPeriod" ,
            "SLA.LitigationRequestFeesNotPaidReminderFirstPeriod" ,
            "Reconciliation.Period" ,
            "SLA.LitigationRequestReconciliationReportResponsePeriod" ,
            "SLA.LitigationRequestReconciliationReportDecisionPeriod" ,
            "Zoom.MeetingDuration" ,
            "SLA.LitigationRequestReformerAssigningRequestDecisionPeriod" ,
            "Reconciliation.ReformerMaxSessionsCountPerDay" ,
            "Reconciliation.ExtendRequest.Added" ,
            "Reconciliation.ExtendRequest.Accepted" ,
            "Reconciliation.ExtendRequest.Rejected" ,
            "Reformer.DeActivate" ,
            "Reformer.Activate" ,
            "SLA.LitigationRequestReformerAssigningRequestResponsePeriod" ,
            "SLA.LitigationRequestExtendRequestReplyPeriod"
        };

        public static readonly List<string> ArbitrationFeesSystemSettings = new List<string>
{
    "Configurable.ArbitratorRegisterationFees.A",
    "Configurable.ArbitratorRegisterationFees.B",
    "Configurable.ArbitratorRegisterationFees.C",
    "Configurable.ArbitratorMembershipFees.A",
    "Configurable.ArbitratorMembershipFees.B",
    "Configurable.ArbitratorMembershipFees.C"
};

        public static readonly List<string> ArbitrationSystemSettings = new List<string>
        {
           "SLA.ArbitratorRegisterationFeesPeriod",
           "SLA.ArbitratorRegisterationFeesFirstNotificationPeriod",
           "SLA.ArbitratorRegisterationFeesNotification",
           "SLA.ArbitratorMembershipFeesPeriod",
           "SLA.ArbitratorMembershipFeesFirstNotificationPeriod",
           "SLA.ArbitratorMembershipFeesNotification",
           "SLA.ArbitratorMembershipRenewalFeesNotification",

           "SLA.LitigationRequestArbitratorCountSelectionResponsePeriod",
           "SLA.LitigationRequestArbitratorCountSelectionDecisionPeriod",
           "SLA.ArbitratorsCount.FirstWarning",
           "SLA.ArbitratorsCount.SecondWarning",
           "SLA.ArbitratorsCount.FinalWarning.Plantiff",
           "SLA.ArbitratorsCount.FinalWarning.Defendant",
           "SLA.ArbitratorsCount.FinalWarning.Both",

           "SLA.LitigationRequestArbitratorAssigningRequestResponseFirstPeriod",
           "SLA.LitigationRequestArbitratorAssigningRequestResponseSecondPeriod",
           "SLA.LitigationRequestArbitratorAssigningRequestDecisionPeriod",
           "SLA.ArbitratorTaskRequest.FirstWarning",
           "SLA.ArbitratorTaskRequest.Cancelled",
           "SLA.ArbitratorChairmanTaskRequest.Cancelled",


            "Arbitration.LitigationRequestArbitratorRecusalRequestPeriod",
            "SLA.LitigationRequestArbitratorNominationListResponsePeriod",
            "SLA.LitigationRequestArbitratorNominationListDecisionPeriod",
            "SLA.AddLitigationRequestArbitratorNominationListPeriod",

            "SLA.LitigationRequestExtendRequestReplyPeriod",

            "Arbitration.Period",

            "SLA.AddLitigationRequestArbitrationSessionPeriod"

        };



        public static readonly List<string> ReconciliationFeesSystemSettings = new List<string>
        {
        "Configurable.ReconciliationFees",
        "Configurable.ReformerJoiningRequestFees",
        };



        public static readonly List<string> ReviewSystemSetting = new List<string>
        {
            "SLA.AssignLitigation.ReviewerAdmin",
            "Review.Period",
            "Review.ReAssigned",
            "Review.Cancelled",
            "SLA.WithdrewLitigation.OldReviewerEmployee",
            "SLA.AssignLitigation.NewReviewerEmployee",
            "SLA.ReminderForReviewer",
            "SLA.ReminderForReviewerManager",
            "SLA.FinishLitigation.ReviewerEmployee",
            "Workflow.Condition.Review",
      //      "SLA.ReviewDelay.ReviewerAdmin",
            "SLA.FinishAgreementRequest",
            "SLA.FirstAgreementRequestForRequest",
            "LitigationRequest.Accept.Agreement",
            "Arbitrators.Count.Added.AfterAgreement",
            "SLA.SecondAgreementRequestForForRequest",
            "SLA.AgreementCancelled",
            "LitigationRequest.Reject.Agreement",
            "LitigationRequest.Creation.Accept.Defendant",
             "LitigationRequest.Creation.Accept",
        };

        public static readonly List<string> ReviewSystemSettingOrder = new List<string>
        {
            "SLA.AssignLitigation.ReviewerAdmin",
            "Review.Period",
            "Review.ReAssigned",
            "Review.Cancelled",
            "SLA.WithdrewLitigation.OldReviewerEmployee",
            "SLA.AssignLitigation.NewReviewerEmployee",
            "SLA.ReminderForReviewer",
            "SLA.ReminderForReviewerManager",
            "SLA.FinishLitigation.ReviewerEmployee",           
            "Arbitrators.Count.Added.AfterAgreement",
            "Workflow.Condition.Review",
            "SLA.FinishAgreementRequest",
            "SLA.FirstAgreementRequestForRequest",
            "LitigationRequest.Accept.Agreement",
            "SLA.SecondAgreementRequestForForRequest",
            "SLA.AgreementCancelled",
            "LitigationRequest.Reject.Agreement",
            "LitigationRequest.Creation.Accept",
            "LitigationRequest.Creation.Accept.Defendant"
        };

    }

}
