using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Invoice;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.ComponentModel.DataAnnotations.Schema;

namespace REAC.EntityFrameworkCore.Entities.Litigation;



public partial class LitigationRequest : BaseEntity<Guid>
{
    [Comment("Request code for the litigation")]
    public string? RequestCode { get; set; }
    [Comment("Current stage of the litigation request")]
    public LitigationRequestStage? LitigationRequestStage { get; set; }

    [Comment("Current sub-stage of the litigation request")]
    public LitigationRequestSubStage? LitigationRequestSubStage { get; set; }

    [Comment("Unique identifier of the litigation request contract")]
    public Guid ContractId { get; set; }
    [Comment("Unique identifier of the city")]
    public Guid? CityId { get; set; }
    [Comment("Unique identifier of the area")]
    public Guid? AreaId { get; set; }
    [Comment("Address")]
    public string? ContractAddress { get; set; } // العنوان الوطني
    [Comment("Unique identifier of the applicant citizen")]
    public Guid? ApplicantCitizenId { get; set; }
    public Guid? DefendantCitizenId { get; set; }
    public Guid? ApplicantFacilityId { get; set; }
    public Guid? DefendantFacilityId { get; set; }

    [Comment("Unique identifier of the delegator citizen")]
    public Guid? DelegatorCitizenId { get; set; }
    [Comment("Unique identifier of the delegator")]
    public Guid? DelegatorId { get; set; }
    [Comment("Unique identifier of the other category")]
    public Guid? OtherCategories { get; set; }
    [Comment("Indicates whether reminder for reviewer was sent")]
    public bool? IsReminderForReviewerSent { get; set; }

    [Comment("Indicates whether reformer reminder for reconciliation first period was sent")]
    public bool? IsReformerReminderForReconciliationFirstPeriodSent { get; set; }

    [Comment("Indicates whether arbitrator count selection response period notification was sent")]
    public bool? IsArbitratorCountSelectionResponsePeriodSent { get; set; }
    [Comment("Indicates whether arbitrator count selection decision period notification was sent")]
    public bool? IsArbitratorCountSelectionDecisionPeriodSent { get; set; }

    public bool? IsArbitratorAdminNominationListAddedPeriodSent { get; set; }
    [Comment("Indicates whether arbitrator session time added period notification was sent")]
    public bool? IsArbitratorSessionTimeAddedPeriodSent { get; set; }
    public bool? IsArbitratorNominationListResponsePeriodSent { get; set; }
    public bool? IsArbitratorNominationListDecisionPeriodSent { get; set; }

    public bool? IsArbitratorChairmanNominationListResponsePeriodSent { get; set; }
    public bool? IsArbitratorChairmanNominationListDecisionPeriodSent { get; set; }


    [Comment("Indicates whether arbitrator assigning request response first period notification was sent")]
    public bool? IsArbitratorAssigningRequestResponseFirstPeriodSent { get; set; }
    [Comment("Indicates whether arbitrator assigning request response second period notification was sent")]
    public bool? IsArbitratorAssigningRequestResponseSecondPeriodSent { get; set; }

    [Comment("Indicates whether litigation request reconciliation report response period notification was sent")]
    public bool? IsLitigationRequestReconciliationReportResponsePeriodSent { get; set; }
    [Comment("Indicates whether litigation request reconciliation report decision period notification was sent")]
    public bool? IsLitigationRequestReconciliationReportDecisionPeriodSent { get; set; }

    [Comment("Indicates whether litigation request reformer assigning request response period notification was sent")]
    public bool? IsLitigationRequestReformerAssigningRequestResponsePeriodSent { get; set; }
    [Comment("Indicates whether reminder for reviewer manager was sent")]
    public bool? IsReminderForReviewerManagerSent { get; set; }

    [Comment("Indicates whether reminder for applicant was sent")]
    public bool? IsReminderForApplicantSent { get; set; }
    [Comment("Indicates whether reminder for agreement request was sent")]
    public bool? IsReminderForAgreementRequestForRequestSent { get; set; }

    [Comment("Indicates whether editing is allowed for this litigation request")]
    public bool IsAllowEdit { get; set; }

    [Comment("Number of first party arbitrators")]
    public int? FirstPartyArbitratorsCount { get; set; }

    [Comment("Number of second party arbitrators")]
    public int? SecondPartyArbitratorsCount { get; set; }
    [Comment("Total number of arbitrators")]
    public int? ArbitratorsCount { get; set; }
    [Comment("Unique identifier of the reformer citizen")]
    public Guid? ReformerCitizenId { get; set; }
    [Comment("Unique identifier of the arbitrator citizen")]
    public Guid? ArbitratorCitizenId { get; set; }

    [Comment("Unique identifier of the litigation request category")]
    public LitigationRequestCategory? LitigationRequestCategoryId { get; set; }

    [Comment("Unique identifier of the litigation request owner type")]
    public LitigationRequestOwnerType LitigationRequestOwnerTypeId { get; set; }
    [Comment("Indicates whether minutes of meeting approval is required for this litigation")]
    public bool? IsMinutesOfMeetingApprovalRequired { get; set; }

    public bool? IsPlaintiffArbitratorNominationListDone { get; set; }
    public bool? IsDefendantArbitratorNominationListDone { get; set; }
    [Comment("Indicates whether second reminder for reconciliation period was sent to reformer")]
    public bool IsReformerReminderForReconciliationSecondPeriodSent { get; set; }
    [Comment("Indicates whether final reminder for reconciliation was sent to reformer")]
    public bool IsReformerFinalReminderForReconciliationSent { get; set; }
    [Comment("Indicates whether reminder for cancel reconciliation was sent to reformer")]
    public bool IsReformerReminderForCancelReconciliationSent { get; set; }
    [Comment("Indicates whether notification to assign new reformer for cancelled reconciliation was sent")]
    public bool IsAssignNewReformerForCancelReconciliationSent { get; set; }
    [Comment("Indicates whether reminder for cancel arbitration was sent to arbitrator")]
    public bool IsArbitratorReminderForCancelArbitrationSent { get; set; }
    [Comment("Indicates whether reminder for cancel litigation due to notes was sent")]
    public bool IsReminderForCancelLitigationForNotesSent { get; set; }
    [Comment("Indicates whether reminder for cancel litigation due to agreement was sent")]
    public bool IsReminderForCancelLitigationForAgreementSent { get; set; }
    [Comment("Indicates whether first reminder for unpaid litigation fees was sent")]
    public bool? IsLitigationRequestFeesNotPaidFirstReminderSent { get; set; }
    [Comment("Indicates whether first reminder for unpaid reconciliation fees was sent")]
    public bool? IsLitigationRequestReconciliationFeesNotPaidFirstReminderSent { get; set; }
    [Comment("Indicates whether first reminder for unpaid arbitration fees was sent")]
    public bool? IsLitigationRequestArbitrationFeesNotPaidFirstReminderSent { get; set; }

    [Comment("Indicates whether agreement request is required for this litigation")]
    public bool? IsAgreementRequestRequired { get; set; }
    [Comment("Date when the agreement request was required")]
    public DateTime? AgreementRequestRequiredOn { get; set; }

    [Comment("Response status of the plaintiff to the agreement request")]
    public ApproveRejectStatus? PlaintiffAgreementRequestResponse { get; set; }
    [Comment("Date when the plaintiff responded to the agreement request")]
    public DateTime? PlaintiffAgreementRequestResponseOn { get; set; }
    [Comment("Response status of the defendant to the agreement request")]
    public ApproveRejectStatus? DefendantAgreementRequestResponse { get; set; }
    [Comment("Date when the defendant responded to the agreement request")]
    public DateTime? DefendantAgreementRequestResponseOn { get; set; }


    [Comment("Approval status of the plaintiff for the minutes of meeting")]
    public ApproveRejectStatus? MintuesOfMeetingApprovalPlaintiffStatus { get; set; }

    [Comment("Approval status of the defendant for the minutes of meeting")]
    public ApproveRejectStatus? MintuesOfMeetingApprovalDefendantStatus { get; set; }
    [Comment("Date when the plaintiff approved/rejected the meeting minutes")]
    public DateTime? MintuesOfMeetingApprovalPlaintiffStatusOn { get; set; }
    [Comment("Date when the defendant approved/rejected the meeting minutes")]
    public DateTime? MintuesOfMeetingApprovalDefendantStatusOn { get; set; }
    [Comment("Status identifier")]
    public RequestStatus InternalStatusId { get; set; }

    [Comment("Status identifier")]
    public RequestStatus ExternalStatusId { get; set; }

    [Comment("Title")]
    public string? Title { get; set; }

    [Comment("Description")]
    public string? Description { get; set; }

    [Comment("Indicates whether this litigation involves financial matters")]
    public bool? IsFinancialLitigation { get; set; }

    [Comment("Unique identifier of the litigation request amount")]
    public Guid? LitigationRequestAmountId { get; set; }

    [Comment("Total fees for this litigation request")]
    public decimal Fees { get; set; } = 0;
    [Comment("Difference between calculated and actual fees")]
    public decimal FeesDifference { get; set; } = 0;
    [Comment("Fees for the reconciliation stage")]
    public decimal ReconciliationFees { get; set; } = 0;
    [Comment("Fees for the arbitration stage")]
    public decimal ArbitrationFees { get; set; } = 0;
    public decimal? DisputeAmount { get; set; } = 0;

    /// <summary>
    [Comment("Indicates whether litigation fees collection request is being sent")]
    public bool IsCollectionOfLitigationFeesRequestIsBeingSent { get; set; } = false;
    [Comment("Indicates whether litigation fees have been collected")]
    public bool IsLitigationFeesHaveBeenCollected { get; set; } = false;
    [Comment("Indicates whether reconciliation fees collection request is being sent")]
    public bool IsCollectionOfReconciliationFeesRequestIsBeingSent { get; set; } = false;
    [Comment("Indicates whether reconciliation fees have been collected")]
    public bool IsReconciliationFeesHaveBeenCollected { get; set; } = false;
    [Comment("Indicates whether arbitration fees collection request is being sent")]
    public bool IsCollectionOfArbitrationFeesRequestIsBeingSent { get; set; } = false;
    [Comment("Indicates whether arbitration fees have been collected")]
    public bool IsArbitrationFeesHaveBeenCollected { get; set; } = false;
    /// </summary>

    [Comment("Duration of the reconciliation period in days")]
    public int? ReconciliationPeriod { get; set; }
    [Comment("Duration of the arbitration period in days")]
    public int? ArbitrationPeriod { get; set; }

    [Comment("Date when reconciliation period starts")]
    public DateTime? StartReconciliationDate { get; set; }
    [Comment("Date when reconciliation period ends")]
    public DateTime? EndReconciliationDate { get; set; }

    [Comment("Date when arbitration period starts")]
    public DateTime? StartArbitrationDate { get; set; }
    [Comment("Date when arbitration period ends")]
    public DateTime? EndArbitrationDate { get; set; }

    [Comment("Indicates whether payment has been made")]
    public bool IsPaid { get; set; } = false;

    [Comment("Date when the payment was made")]
    public DateTime? PaidOn { get; set; }

    [Comment("Date and time when the record was created")]
    public DateTime CreatedOn { get; set; }

    [Comment("Indicates whether the litigation request has been sent")]
    public bool IsSent { get; set; }

    [Comment("Date when the litigation request was sent")]
    public DateTime? SentOn { get; set; }
    [Comment("Indicates whether the record is active")]
    public bool? IsActive { get; set; }

    [Comment("Indicates whether the litigation request has been completed")]
    public bool? IsFinished { get; set; }

    [Comment("Indicates whether exemption reduction applies to this litigation")]
    public bool? IsExemptionReduction { get; set; }

    [Comment("Duration")]
    public int? ArbitratorSelectionDuration { get; set; }

    [Comment("Unique identifier of the exemption reduction category")]
    public Guid? ExemptionReductionCategoryId { get; set; }

    [Comment("Unique identifier of the exemption reduction file")]
    public Guid? ExemptionReductionFileId { get; set; }

    public virtual SysFileManager? ExemptionReductionFile { get; set; }


    [Comment("Rating given by the plaintiff for the litigation process")]
    public int? PretenderRating { get; set; }
    [Comment("Rating given by the defendant for the litigation process")]
    public int? DefendantRating { get; set; }


    /// <summary>
    /// Actions Times
    /// </summary>
    [Comment("Duration")]
    public DateTime? ArbitratorsSelectionDurationDateTime { get; set; }
    [Comment("Date and time when arbitrators count was determined")]
    public DateTime? ArbitratorsCountDateTime { get; set; }
    public DateTime? ArbitratorsNominationListCreationDateTime { get; set; }
    public DateTime? ArbitratorsNominationListSelectionDoneDateTime { get; set; }
    public DateTime? ArbitratorsChairmanNominationListCreationDateTime { get; set; }
    public DateTime? ArbitratorsChairmanNominationListSelectionDoneDateTime { get; set; }
    [Comment("Date and time of the note")]
    public DateTime? JudgmentDecisionIssuingDateTime { get; set; }



    [Comment("Indicates whether arbitrators chairman decision is required for this litigation")]
    public bool? IsArbitratorsChairmanDecisionRequired { get; set; }

    public virtual Citizen? DelegatorCitizen { get; set; }

    [ForeignKey(nameof(DelegatorId))]
    public virtual Delegator? Delegator { get; set; }

    public virtual Citizen ApplicantCitizen { get; set; } = null!;

    [ForeignKey(nameof(DefendantCitizenId))]
    public virtual Citizen DefendantCitizen { get; set; } = null!;
    
    [ForeignKey(nameof(DefendantFacilityId))]
    public virtual Facility DefendantFacility { get; set; } = null!;
    
    [ForeignKey(nameof(ApplicantFacilityId))]
    public virtual Facility ApplicantFacility { get; set; } = null!;
    public virtual SysCity City { get; set; } = null!;
    public virtual SysArea Area { get; set; } = null!;
    public virtual Citizen? ReformerCitizen { get; set; }
    public virtual Citizen? ArbitratorCitizen { get; set; }

    public virtual LitigationRequestContract Contract { get; set; } = null!;

    public virtual SysLitigationRequestStatus ExternalStatus { get; set; } = null!;

    public virtual SysLitigationRequestStatus InternalStatus { get; set; } = null!;

    public virtual SysLitigationRequestAmount LitigationRequestAmount { get; set; } = null!;

    public virtual SysLitigationRequestCategory? LitigationRequestCategory { get; set; }

    public virtual SysExemptionReductionCategory? SysExemptionReductionCategory { get; set; }
    public virtual OtherCategory? OtherCategory { get; set; }
    public virtual ICollection<LitigationRequestSession> LitigationRequestSessions { get; set; } = new List<LitigationRequestSession>();

    public virtual ICollection<LitigationRequestReformerAssigningRequest> LitigationRequestReformerAssigningRequests { get; set; } = new List<LitigationRequestReformerAssigningRequest>();
    public virtual ICollection<LitigationRequestArbitratorAssigningRequest> LitigationRequestArbitratorAssigningRequests { get; set; } = new List<LitigationRequestArbitratorAssigningRequest>();

    public virtual ICollection<LitigationRequestCorrespondence> LitigationRequestCorrespondences { get; set; } = new List<LitigationRequestCorrespondence>();

    public virtual ICollection<LitigationRequestExtendPeriodRequest> LitigationRequestExtendPeriodRequests { get; set; } = new List<LitigationRequestExtendPeriodRequest>();
    public virtual ICollection<LitigationRequestArbitrationReport> LitigationRequestArbitrationReports { get; set; } = new List<LitigationRequestArbitrationReport>();
    public virtual ICollection<LitigationRequestArbitratorRecusalRequest> LitigationRequestArbitratorRecusalRequests { get; set; } = new List<LitigationRequestArbitratorRecusalRequest>();

    public virtual ICollection<LitigationRequestGround> LitigationRequestGrounds { get; set; } = new List<LitigationRequestGround>();

    public virtual ICollection<LitigationRequestReconciliationReport> LitigationRequestReconciliationReports { get; set; } = new List<LitigationRequestReconciliationReport>();

    public virtual ICollection<LitigationRequestNote> LitigationRequestNotes { get; set; } = new List<LitigationRequestNote>();
    public virtual ICollection<LititgationRequestModificationRequest> LitigationRequestModificationRequests { get; set; } = new List<LititgationRequestModificationRequest>();
    public virtual ICollection<LitigationRequestArbitratorList> LitigationRequestArbitratorLists { get; set; } = new List<LitigationRequestArbitratorList>();
    public virtual ICollection<LitigationRequestArbitratorNominationList> LitigationRequestArbitratorNominationLists { get; set; } = new List<LitigationRequestArbitratorNominationList>();
    public virtual ICollection<LitigationRequestArbitratorsChairmanNominationList> LitigationRequestArbitratorsChairmanNominationLists { get; set; } = new List<LitigationRequestArbitratorsChairmanNominationList>();



    public virtual SysLitigationRequestOwnerType LitigationRequestOwnerType { get; set; } = null!;
    public virtual SysInvoice LitigationRequestInvoice { get; set; } = null!;

    public virtual ICollection<LitigationRequestRequest> LitigationRequestRequests { get; set; } = new List<LitigationRequestRequest>();
    public virtual ICollection<WorkFlowProcessingLog> WorkFlowProcessingLogs { get; set; } = new List<WorkFlowProcessingLog>();

    [InverseProperty(nameof(DelegationRequest.LitigationRequest))]
    public virtual ICollection<DelegationRequest> LitigationDelegationRequests { get; set; } = new List<DelegationRequest>();

}




