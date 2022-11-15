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
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> PostRegister(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email,
                BirthDate = request.BirthDate
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password!);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
