namespace REAC.Application.Contracts.IAppServices.Integration.Hubs.Contracts
{
    public interface IIdentityClient
    {
        Task ReceivedIdentityStatus(IdentityStatus identityStatus);
    }
}
