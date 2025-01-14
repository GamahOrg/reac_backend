namespace REAC.Domain.Shared.Enum
{
    public enum LitigationRequestSubStage
    {
        AddSession = 0,
        LitigationRequestReportAdded = 1,
        LitigationRequestPartiesDecisionAdded = 2,
        ArbitratorSelectionPeriod = 3,
        SetArbitratorsNumber = 4,
        PrepareArbitratorsListAndSendTheNominationRequest = 5,
        SetReformerFirstTime = 6,
        LitigationRequestReformerDecisionAdded = 7,
        LitigationRequestAgreementRequestRequired = 8,


        ArbitratorSelectionDuration = 9,
        LitigationRequestAddArbitratorCount = 10,

        LitigationRequestAddArbitratorListNomination = 11,
        LitigationRequestAddArbitratorListNominationDone = 12,

        LitigationRequestAddArbitratorChairmanListNomination = 13,
        LitigationRequestAddArbitratorChairmanListNominationDone = 14,

        LitigationRequestAddArbitrationSession = 15,

        JudgmentDecisionIssuing = 16,
        JudgmentDecisionIssued = 17,



    }
}
