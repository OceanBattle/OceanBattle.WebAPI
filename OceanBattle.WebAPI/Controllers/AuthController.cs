using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;
using OceanBattle.Jwks.Abstractions;
using OceanBattle.Jwt.Abstractions;
using OceanBattle.RefreshTokens.Abstractions;
using OceanBattle.RefreshTokens.DataModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        private readonly IJwtService _jwtService;
        private readonly IJwksFactory _jwksFactory;
        private readonly IRefreshTokenFactory _refreshTokenFactory;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthController(
            ILogger<AuthController> logger,
            UserManager<User> userManager,
            IJwtFactory jwtFactory,
            IJwtService jwtService,
            IJwksFactory jwksFactory,
            IRefreshTokenFactory refreshTokenFactory,
            IRefreshTokenService refreshTokenService)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtService = jwtService;
            _jwksFactory = jwksFactory;
            _refreshTokenFactory = refreshTokenFactory;
            _refreshTokenService = refreshTokenService;
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

            AuthResponse response = await CreateAuthResponse(user);

            return Ok(response);
        }

        /// <summary>
        /// Refreshes user's JSON Web Token.
        /// </summary>
        /// <param name="request">Request model body containing refresh token and JWT.</param>
        /// <returns><see cref="AuthResponse"/> with new tokens.</returns>
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> PostRefresh(TokenRefreshRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            TokenValidationResult bearerTokenResult =
                await _jwtService.ValidateExpiredTokenAsync(request.BearerToken!);

            if (!bearerTokenResult.IsValid)
                return Unauthorized();

            User? user =
                await _userManager.GetUserAsync(new ClaimsPrincipal(bearerTokenResult.ClaimsIdentity));

            if (user is null)
                return Unauthorized();

            PasswordVerificationResult refreshTokenResult =
                await _refreshTokenService.ValidateTokenAsync(request.RefreshToken!, user);

            if (refreshTokenResult is PasswordVerificationResult.Failed)
                return Unauthorized();

            AuthResponse authResponse = await CreateAuthResponse(user);

            return Ok(authResponse);
        }

        /// <summary>
        /// Logs user out.
        /// </summary>
        /// <returns><see cref="IActionResult"/> of log out.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> PostLogOut()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? user = await _userManager.GetUserAsync(User);

            if (user is null)
                return BadRequest();

            await _refreshTokenService.RevokeTokenAsync(user);
            await _jwtService.BlacklistTokenAsync(
                new JwtSecurityToken(await HttpContext.GetTokenAsync("access_token")));

            return Ok();
        }

        /// <summary>
        /// Endpoint exposing public RSA keys for JWT authentication.
        /// </summary>
        /// <returns><see cref="IActionResult"/> containing JSON Web Key Set.</returns>
        [AllowAnonymous]
        [HttpGet(".well-known")]
        public IActionResult GetWellKnown() =>
            Ok(new
            {
                keys = _jwksFactory.GetPublicKeys()
            });


        #region private helpers

        /// <summary>
        /// Creates auth response instance.
        /// </summary>
        /// <param name="user"><see cref="User"/> that <see cref="AuthResponse"/> is generated for.</param>
        /// <returns><see cref="AuthResponse"/> instance.</returns>
        private async Task<AuthResponse> CreateAuthResponse(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            RefreshToken refreshToken = _refreshTokenFactory.CreateToken(user);

            await _refreshTokenService.UpdateTokenAsync(refreshToken);

            AuthResponse response = new AuthResponse
            {
                BearerToken = tokenHandler.WriteToken(_jwtFactory.CreateToken(user)),
                RefreshToken = refreshToken.Token
            };

            return response;
        }

        #endregion
    }
}
