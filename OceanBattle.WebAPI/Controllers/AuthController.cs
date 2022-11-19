using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;
using OceanBattle.Jwt.Abstractions;
using System.IdentityModel.Tokens.Jwt;

namespace OceanBattle.Controllers
{
    /// <summary>
    /// Controller hadling authentication and authorization operations.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;

        public AuthController(
            ILogger<AuthController> logger,
            UserManager<User> userManager,
            IJwtFactory jwtFactory)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
        }

        /// <summary>
        /// Logs user in.
        /// </summary>
        /// <param name="request">Request model body containing log in credentials.</param>
        /// <returns>Auth response <see cref="Task{ActionResult{AuthResponse}}"/>.</returns>
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

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanWriteToken)
                return Problem();

            AuthResponse response = new AuthResponse
            {
                BearerToken = tokenHandler.WriteToken(_jwtFactory.CreateToken(user))
            };

            return Ok(response);
        }
    }
}
