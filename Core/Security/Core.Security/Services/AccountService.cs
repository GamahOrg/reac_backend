using Core.Dtos;
using Core.Security.Contracts;
using Core.Security.Entities;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Security.Services
{
    public class AccountService : IAccountService
    {
        private UserManager<User> _userManager;
        private readonly IJwtProvider _jwtProvider;

        public AccountService(UserManager<User> userManager, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
        }

        public async Task<DataResponse<TokenModel>> LoginAsync(UserLoginDto userLogin, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(userLogin.UserName))
            {
                string userName = userLogin.UserName.Trim().ToLower();
                User user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName || x.Email == userName, cancellationToken);

                if (user != null)
                {
                    var isValidPassword = await _userManager.CheckPasswordAsync(user, userLogin.Password);
                    if (isValidPassword)
                    {
                        var authentication = await _jwtProvider.GenerateAuthenticationTokens(user);
                        //if (!authentication.ReferenceId.HasValue)
                        //    return new DataResponse<TokenModel> { IsDone = true, Data = null, Message = "ReferenceId is null" };

                        user.RefreshToken = authentication.Token.RefreshToken;
                        user.RefreshTokenExpiryTime = authentication.RefreshTokenExpiration;

                        var identityResult = await _userManager.UpdateAsync(user);
                        if (identityResult != null && identityResult.Succeeded)
                        {
                            return new DataResponse<TokenModel> { Data = authentication.Token, IsDone = true };
                        }
                        else
                        {
                            return new DataResponse<TokenModel> { IsDone = false, Data = null, Message = "خطأ فى رقم الهوية" };
                        }
                    }
                    else
                    {
                        return new DataResponse<TokenModel> { IsDone = false, Data = null, Message = "خطأ فى رقم الهوية" };
                    }
                }
                else
                {
                    return new DataResponse<TokenModel> { IsDone = false, Data = null, Message = "خطأ فى رقم الهوية" };
                }

            }
            else
            {
                return new DataResponse<TokenModel> { IsDone = false, Data = null, Message = "Invalid User Name Or Password!" };
            }
        }


        public async Task<BaseResponse> RefreshTokenAsync(TokenModel tokenModel, CancellationToken cancellationToken)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == tokenModel.RefreshToken, cancellationToken);
            if (user != null)
            {
                if (user.RefreshTokenExpiryTime > DateTime.Now)
                {
                    var authentication = await _jwtProvider.GenerateAuthenticationTokens(user);

                    user.RefreshToken = authentication.Token.RefreshToken;
                    user.RefreshTokenExpiryTime = authentication.RefreshTokenExpiration;
                    var identityResult = await _userManager.UpdateAsync(user);
                    if (identityResult != null && identityResult.Succeeded)
                    {
                        return new DataResponse<TokenModel> { Data = authentication.Token, IsDone = true };
                    }
                    else
                    {
                        return new DataResponse<TokenModel> { IsDone = false, Data = null, Message = "Failed to generate new token" };
                    }
                }
                else
                {
                    return new DataResponse<TokenModel> { IsDone = false, Data = null, Message = "Refresh Token is expired" };
                }
            }
            else
            {
                return new DataResponse<TokenModel> { IsDone = false, Data = null, Message = "Invalid Refresh Token" };
            }


        }

    }
}
