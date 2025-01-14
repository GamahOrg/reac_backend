using Core.Security.Dtos;
using Core.Shared.Responses;

namespace Core.Security.Contracts
{
    public interface IUserService
    {
        Task<BaseResponse> AddAsync(UserDto userDto, CancellationToken cancellationToken);
    }
}
