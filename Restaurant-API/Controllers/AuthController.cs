using Microsoft.AspNetCore.Mvc;
using Restaurant_API.Attributes;
using Restaurant_API.Data.Models.Auth;
using Restaurant_API.Data.Models.User;
using Restaurant_API.Services.Interfaces;

namespace Restaurant_API.Controllers
{
    [ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly IUserService userService;
		private readonly ISessionService sessionService;

		public AuthController(IUserService userService, ISessionService sessionService)
		{
			this.userService = userService;
			this.sessionService = sessionService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> RegisterAsync(UserRegisterModel registerModel)
		{
			UserAuthModel auth = await userService.RegisterAsync(registerModel);
			return Ok(auth);
		}

		[HttpPost("login")]
		public async Task<IActionResult> LoginAsync(UserLoginModel loginModel)
		{
            UserAuthModel auth = await userService.LoginAsync(loginModel);
			return Ok(auth);
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> RefreshAsync(RefreshTokenRequest refreshModel)
		{
			UserAuthModel auth = await sessionService.RefreshSessionAsync(refreshModel.RefreshToken);
			return Ok(auth);
		}

		[HttpPost("logout"), AppAuthorize]
		public async Task<IActionResult> LogoutAsync()
		{
			await sessionService.EndCurrSessionAsync(HttpContext);
			return NoContent();
		}
	}
}
