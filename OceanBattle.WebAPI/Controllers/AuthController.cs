using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;

namespace OceanBattle.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<User> _userManager;

        public AuthController(
            ILogger<AuthController> logger,
            UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> PostLogIn(LogInRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? user = await _userManager.FindByEmailAsync(request.Email!);

            if (user is null)
                return Unauthorized("Invalid E-Mail.");

            if (!await _userManager.CheckPasswordAsync(user, request.Password!))
                return Unauthorized("Invalid Password.");

            return Ok();
        }
    }
}
