using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.EntityFrameworkCore.Entities.Litigation;



public partial class LitigationRequest : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public LitigationRequestStage? LitigationRequestStage { get; set; }

    public LitigationRequestSubStage? LitigationRequestSubStage { get; set; }

    public Guid ContractId { get; set; }
    public Guid? CityId { get; set; }
    public Guid? AreaId { get; set; }
    public string? ContractAddress { get; set; } // العنوان الوطني
    public Guid ApplicantCitizenId { get; set; }

    public Guid? DelegatorCitizenId { get; set; }
    public Guid? OtherCategories { get; set; }
    public bool? IsReminderForReviewerSent { get; set; }

    public bool? IsReformerReminderForReconciliationFirstPeriodSent { get; set; }

    public bool? IsArbitratorCountSelectionResponsePeriodSent { get; set; }
    public bool? IsArbitratorCountSelectionDecisionPeriodSent { get; set; }

    public bool? IsArbitratorNominationListResponsePeriodSent { get; set; }
    public bool? IsArbitratorNominationListDecisionPeriodSent { get; set; }

    public bool? IsArbitratorChairmanNominationListResponsePeriodSent { get; set; }
    public bool? IsArbitratorChairmanNominationListDecisionPeriodSent { get; set; }


    public bool? IsArbitratorAssigningRequestResponseFirstPeriodSent { get; set; }
    public bool? IsArbitratorAssigningRequestResponseSecondPeriodSent { get; set; }

    public bool? IsLitigationRequestReconciliationReportResponsePeriodSent { get; set; }

    public bool? IsLitigationRequestReformerAssigningRequestResponsePeriodSent { get; set; }
    public bool? IsReminderForReviewerManagerSent { get; set; }

    public bool? IsReminderForApplicantSent { get; set; }
    public bool? IsReminderForAgreementRequestForRequestSent { get; set; }

    public bool IsAllowEdit { get; set; }

    public int? FirstPartyArbitratorsCount { get; set; }

    public int? SecondPartyArbitratorsCount { get; set; }
    public int? ArbitratorsCount { get; set; }
    public Guid? ReformerCitizenId { get; set; }
    public Guid? ArbitratorCitizenId { get; set; }

    public LitigationRequestCategory? LitigationRequestCategoryId { get; set; }

    public LitigationRequestOwnerType LitigationRequestOwnerTypeId { get; set; }
    public bool? IsMinutesOfMeetingApprovalRequired { get; set; }

    public bool? IsPlaintiffArbitratorNominationListDone { get; set; }
    public bool? IsDefendantArbitratorNominationListDone { get; set; }

    public bool? IsAgreementRequestRequired { get; set; }
    public DateTime? AgreementRequestRequiredOn { get; set; }

    public ApproveRejectStatus? PlaintiffAgreementRequestResponse { get; set; }
    public DateTime? PlaintiffAgreementRequestResponseOn { get; set; }
    public ApproveRejectStatus? DefendantAgreementRequestResponse { get; set; }
    public DateTime? DefendantAgreementRequestResponseOn { get; set; }


    public ApproveRejectStatus? MintuesOfMeetingApprovalPlaintiffStatus { get; set; }

    public ApproveRejectStatus? MintuesOfMeetingApprovalDefendantStatus { get; set; }
    public DateTime? MintuesOfMeetingApprovalPlaintiffStatusOn { get; set; }
    public DateTime? MintuesOfMeetingApprovalDefendantStatusOn { get; set; }
    public RequestStatus InternalStatusId { get; set; }

    public RequestStatus ExternalStatusId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool? IsFinancialLitigation { get; set; }

    public Guid? LitigationRequestAmountId { get; set; }

    public decimal Fees { get; set; } = 0;
    public decimal ReconciliationFees { get; set; } = 0;
    public decimal ArbitrationFees { get; set; } = 0;

    /// <summary>
    public bool IsCollectionOfLitigationFeesRequestIsBeingSent { get; set; } = false;
    public bool IsLitigationFeesHaveBeenCollected { get; set; } = false;
    public bool IsCollectionOfReconciliationFeesRequestIsBeingSent { get; set; } = false;
    public bool IsReconciliationFeesHaveBeenCollected { get; set; } = false;
    public bool IsCollectionOfArbitrationFeesRequestIsBeingSent { get; set; } = false;
    public bool IsArbitrationFeesHaveBeenCollected { get; set; } = false;
    /// </summary>

    public int? ReconciliationPeriod { get; set; }
    public int? ArbitrationPeriod { get; set; }

    public DateTime? StartReconciliationDate { get; set; }
    public DateTime? EndReconciliationDate { get; set; }

    public DateTime? StartArbitrationDate { get; set; }
    public DateTime? EndArbitrationDate { get; set; }

    public bool IsPaid { get; set; } = false;

    public DateTime? PaidOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsSent { get; set; }

    public DateTime? SentOn { get; set; }
    public bool? IsActive { get; set; }

    public bool? IsFinished { get; set; }

    public bool? IsExemptionReduction { get; set; }

    public int? ArbitratorSelectionDuration { get; set; }

    public Guid? ExemptionReductionCategoryId { get; set; }

    public Guid? ExemptionReductionFileId { get; set; }

    public virtual SysFileManager? ExemptionReductionFile { get; set; }


    public int? PretenderRating { get; set; }
    public int? DefendantRating { get; set; }


    /// <summary>
    /// Actions Times
    /// </summary>
    public DateTime? ArbitratorsSelectionDurationDateTime { get; set; }
    public DateTime? ArbitratorsCountDateTime { get; set; }
    public DateTime? ArbitratorsNominationListCreationDateTime { get; set; }
    public DateTime? ArbitratorsNominationListSelectionDoneDateTime { get; set; }
    public DateTime? ArbitratorsChairmanNominationListCreationDateTime { get; set; }
    public DateTime? ArbitratorsChairmanNominationListSelectionDoneDateTime { get; set; }
    public DateTime? JudgmentDecisionIssuingDateTime { get; set; }



    public bool? IsArbitratorsChairmanDecisionRequired { get; set; }

    public virtual Citizen? DelegatorCitizen { get; set; }

    public virtual Citizen ApplicantCitizen { get; set; } = null!;
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

    public virtual ICollection<LitigationRequestRequest> LitigationRequestRequests { get; set; } = new List<LitigationRequestRequest>();
    public virtual ICollection<WorkFlowProcessingLog> WorkFlowProcessingLogs { get; set; } = new List<WorkFlowProcessingLog>();

}


