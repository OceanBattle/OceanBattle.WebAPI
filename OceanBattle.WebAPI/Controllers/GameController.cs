using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Services;

namespace OceanBattle.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILevelsRepository _levelsRepository;
        private readonly UserManager<User> _userManager;
        private readonly IPlayersManager _playersManager;

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
        public async Task<ActionResult<IEnumerable<LevelDto>>> GetLevels()
        {
            IEnumerable<Level> levels = _levelsRepository.GetLevels();
            IEnumerable<LevelDto> levelDtos = levels.Select(l => new LevelDto
            {
                BattlefieldSize = l.BattlefieldSize,
                AvailableTypes = l.AvailableTypes is null ? null :
                l.AvailableTypes.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value)
            });

            return Ok(levelDtos);
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
