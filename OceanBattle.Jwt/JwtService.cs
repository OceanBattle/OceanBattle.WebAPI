using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OceanBattle.Jwt.Abstractions;
using OceanBattle.Jwt.Helpers;
using OceanBattle.RefreshTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OceanBattle.Jwt
{
    /// <summary>
    /// Service for performing basic operations on JSON Web Tokens.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IDistributedCache _cache;
        private readonly RefreshTokenOptions _refreshTokenOptions;
        private readonly JwtOptions _jwtOptions;

        public JwtService(
            IOptionsMonitor<JwtBearerOptions> options,
            IDistributedCache cache,
            IOptions<RefreshTokenOptions> refreshTokenOptions,
            IOptions<JwtOptions> jwtOptions)
        {
            _tokenValidationParameters = 
                options.Get(JwtBearerDefaults.AuthenticationScheme)
                       .TokenValidationParameters;

            _cache = cache;
            _refreshTokenOptions = refreshTokenOptions.Value;
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// Adds JTI claim of requested JSON Web Token to blacklist (this token will no longer be recognized as valid).
        /// </summary>
        /// <param name="jti">Id of JSON Web Token to be added to blacklist.</param>
        /// <returns><see cref="Task"/> representing <see langword="async"/> operation.</returns>
        public async Task BlacklistTokenAsync(Guid jti)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = 
                    _refreshTokenOptions.Expires.Add(_jwtOptions.Expires)
            };

            await _cache.SetStringAsync(
                string.Format("{0}{1}", StringHelpers.BlacklistPath, jti.ToString("N")), 
                jti.ToString(), 
                cacheOptions);
        }

        /// <summary>
        /// Manually validates JSON Web Token that can be expired (ignores token lifetime).
        /// </summary>
        /// <param name="token">String representing JSON Web Token.</param>
        /// <returns><see cref="TokenValidationResult"/> of validation.</returns>
        public async Task<TokenValidationResult> ValidateExpiredTokenAsync(string token)
        {
            TokenValidationParameters validationParameters = 
                _tokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false;

            var tokenHandler = new JwtSecurityTokenHandler();

            var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);

            if (result.IsValid)
                result.IsValid = !(await IsTokenBlacklistedAsync((new JwtSecurityToken(token)).Claims.ToList()));
            
            return result;
        }

        /// <summary>
        /// Manually validates JSON Web Token.
        /// </summary>
        /// <param name="token">String representing JSON Web Token.</param>
        /// <returns><see cref="TokenValidationResult"/> of validation.</returns>
        public async Task<TokenValidationResult> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var result = await tokenHandler.ValidateTokenAsync(token, _tokenValidationParameters);

            if (result.IsValid)
                result.IsValid = !(await IsTokenBlacklistedAsync((new JwtSecurityToken(token)).Claims.ToList()));

            return result;
        }

        /// <summary>
        /// Verifies against the blacklist if JSON Web Token is blacklisted.
        /// </summary>
        /// <param name="claims">Token claims.</param>
        /// <returns><see langword="true"/> if it is blacklisted, <see langword="false"/> if it is not.</returns>
        public async Task<bool> IsTokenBlacklistedAsync(List<Claim> claims)
        {
            Claim? claim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

            return claim is not null && 
                await _cache.GetAsync(string.Format("{0}{1}", StringHelpers.BlacklistPath, claim.Value)) != null;      
        }
    }
}
