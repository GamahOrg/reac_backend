using Core.Security.Dtos;
using Core.Security.Entities;
using Core.Shared.Responses;

namespace Core.Security.Contracts
{
    public interface IJwtProvider
    {
        /// <summary>
        /// Generate JWT Bearer token for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<AuthDto> GenerateAuthenticationTokens(User user);

        Task<BaseResponse> ValidateActiveToken();
        Task<BaseResponse> ValidateActiveToken(string token);
    }
}
