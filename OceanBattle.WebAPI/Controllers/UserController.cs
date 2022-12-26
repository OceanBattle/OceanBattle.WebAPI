using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OceanBattle.Data;
using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;
using OceanBattle.Game.Abstractions;

namespace OceanBattle.Controllers
{
    /// <summary>
    /// Controller handling operations on users (eg. CRUD operations).
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly IShipsRepository _shipsRepository;

        public UserController(
            UserManager<User> userManager,
            AppDbContext dbContext,
            IShipsRepository shipsRepository)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _shipsRepository = shipsRepository;
        }

        /// <summary>
        /// Registers new user and adds to database.
        /// </summary>
        /// <param name="request">Request model body containing informations about user.</param>
        /// <returns><see cref="Task{IActionResult}"/> response.</returns>
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
                OwnedVessels = _shipsRepository.GetShips()
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password!);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Gets user data transfer object filled with <see cref="User"/>'s data.
        /// </summary>
        /// <returns><see cref="UserDto"/> with <see cref="User"/>'s data.</returns>
        [HttpGet("user")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string id = _userManager.GetUserId(User)!;
            User user = (await _dbContext.Users.FindAsync(id))!;

            UserDto dto = new UserDto
            {
                Email = user.Email,
                UserName = user.UserName
            };

            return Ok(dto);
        }
    }
}
