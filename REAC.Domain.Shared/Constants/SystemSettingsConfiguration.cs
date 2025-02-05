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
    };

        public static readonly List<string> ReconciliationFeesSystemSettings = new List<string>
    {
        "Configurable.ReconciliationFees",
        "Configurable.ReformerJoiningRequestFees",
    };

    }

}
