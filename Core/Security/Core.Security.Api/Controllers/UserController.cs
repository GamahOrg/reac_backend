using Core.Security.Contracts;
using Core.Security.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Core.Security.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost(nameof(Create))]
        public async Task<IActionResult> Create([FromBody] UserDto user, CancellationToken cancellationToken)
            => Ok(await _userService.AddAsync(user, cancellationToken));
    }
}
