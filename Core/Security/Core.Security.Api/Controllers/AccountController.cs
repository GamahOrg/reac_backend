using Core.Dtos;
using Core.Security.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Security.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtProvider _jwtProvider;

        public AccountController(IAccountService accountService, IJwtProvider jwtProvider)
        {
            _accountService = accountService;
            _jwtProvider = jwtProvider;
        }

        [HttpPost(nameof(Login)), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto UserLogin, CancellationToken cancellationToken)
            => Ok(await _accountService.LoginAsync(UserLogin, cancellationToken));


        [HttpPost(nameof(RefreshToken)), AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel, CancellationToken cancellationToken)
           => Ok(await _accountService.RefreshTokenAsync(tokenModel, cancellationToken));

        [HttpGet(nameof(ValidateActiveToken))]
        public async Task<IActionResult> ValidateActiveToken()
            => Ok(await _jwtProvider.ValidateActiveToken());
    }
}
