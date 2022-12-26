using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Abstractions;

namespace OceanBattle.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IPlayersManager _playersManager;
        private readonly ILevelsRepository _levelsRepository;

        public GameController(
            UserManager<User> userManager,
            IPlayersManager playersManager,
            ILevelsRepository levelsRepository) 
        {
            _userManager = userManager;
            _playersManager = playersManager;
            _levelsRepository = levelsRepository;
        }

        /// <summary>
        /// Gets list of available levels.
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="Level"/>.</returns>
        [HttpGet("levels")]
        public ActionResult<IEnumerable<Level>> GetLevels()
        {
            return Ok(_levelsRepository.GetLevels());
        }

        /// <summary>
        /// Gets list of active users.
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of active <see cref="UserDto"/>.</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetActive()
        {
            User? user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();

            return Ok(_playersManager.ActivePlayers
                .Where(u => u.Id != user.Id)
                .Select(u => new UserDto 
                {
                    Email = u.Email,
                    UserName = u.UserName
                }));
        }
    }
}
