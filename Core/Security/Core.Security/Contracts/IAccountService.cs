using Core.Dtos;
using Core.Security.Dtos;
using Core.Shared.Responses;

namespace Core.Security.Contracts
{
    public interface IAccountService
    {
        Task<DataResponse<TokenModel>> LoginAsync(UserLoginDto UserLogin, CancellationToken cancellationToken);
        Task<BaseResponse> RefreshTokenAsync(TokenModel tokenModel, CancellationToken cancellationToken);
    }
}
