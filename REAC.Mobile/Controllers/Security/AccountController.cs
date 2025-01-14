using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.AppServices.Integration.GEE;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.UserManage;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Domain.Shared.Constants;

namespace REAC.Mobile.Controllers.Security
{
    [Route("api/mobile/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Security)]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly INafathService _nafathService;
        private readonly IUserRegisterService _userRegister;
        private readonly INafathTransactionService _nafathTransactionService;

        public AccountController(INafathService nafathService, IUserRegisterService userRegister, INafathTransactionService nafathTransactionService)
        {
            _nafathService = nafathService;
            _userRegister = userRegister;
            _nafathTransactionService = nafathTransactionService;
        }

        [HttpPost(nameof(VerifyUser))]
        public async Task<IActionResult> VerifyUser(NafathRequestDto nafathRequestDto, CancellationToken cancellationToken)
        {
            return Ok(await _nafathService.Nafath(nafathRequestDto, cancellationToken));
        }

        [HttpPost(nameof(Register))]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            return Ok(await _userRegister.Register(userRegisterDto));
        }
        [HttpGet(nameof(CheckStatus))]
        public async Task<IActionResult> CheckStatus(string transactionId)
        {
            return Ok(await _nafathService.CheckStatusForMobile(transactionId));
        }

        [HttpGet(nameof(GetNafathTransactionList))]
        public async Task<IActionResult> GetNafathTransactionList()
        {
            return Ok(_nafathTransactionService.GettAll());
        }
    }
}
