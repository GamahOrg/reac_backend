namespace REAC.Application.Contracts.Dtos.Arbitration
{
    public class ArbitratorCountRequestPayload
    {
        public Guid RequestId { get; set; }
        public int ArbitratorCount { get; set; }
    }
    public class ArbitratorNominationListRequestPayload
    {
        public Guid RequestId { get; set; }
        public List<Guid> Arbitrators { get; set; } = new List<Guid>();
    }
    public class ArbitratorNominationListRequestReplyPayload
    {
        public Guid RequestId { get; set; }

        public List<ArbitratorNominationListRequestItem> ArbitratorNominationListRequestItems { get; set; } = new List<ArbitratorNominationListRequestItem>();
    }

    public class ArbitratorNominationListRequestItem
    {
        public Guid ArbitratorListId { get; set; }
        public int Priority { get; set; }
    }
}

