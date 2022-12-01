using System.Threading.Tasks;
using Customer.Portal.Web.Services;
using Customer.Portal.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Portal.Web.Controllers {
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel) {
            return Ok(await _authService.Login(viewModel));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateViewModel viewModel) {
            await _authService.Register(viewModel);
            return Ok();
        }
    }
}