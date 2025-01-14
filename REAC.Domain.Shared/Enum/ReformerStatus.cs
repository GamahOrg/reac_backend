namespace REAC.Domain.Shared.Enum
{
    public enum ReformerStatus
    {
        Certified = 1,
        Stopped = 2
    }
    public enum ArbitratorStatus
    {
        Certified = 1,
        Stopped = 2
    }
    public enum StoppingReason
    {
        BadRating = 1,
        LateLitigationRequest = 2,
        NonPaymentOfMembershipFees = 3
    }
}
