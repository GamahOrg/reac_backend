using REAC.Application.Contracts.Dtos.Integration.GEE;

namespace REAC.Application.Contracts.IAppServices.Integration.Hubs.Contracts
{
    public class IdentityStatus
    {
        public string TransactionId { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsValid { get; set; }
        public UserInfoUpdatedDto? UserInfoUpdated { get; set; }
        public List<UserAddressInfoUpdatedDto>? UserAddressInfoUpdated { get; set; }
    }
}
