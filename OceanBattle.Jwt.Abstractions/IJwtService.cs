using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Jwt.Abstractions
{
    /// <summary>
    /// Service for performing basic operations on JSON Web Tokens.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Manually validates JSON Web Token.
        /// </summary>
        /// <param name="token">String representing JSON Web Token.</param>
        /// <returns><see cref="TokenValidationResult"/> of validation.</returns>
        Task<TokenValidationResult> ValidateTokenAsync(string token);

        /// <summary>
        /// Manually validates JSON Web Token that can be expired (ignores token lifetime).
        /// </summary>
        /// <param name="token">String representing JSON Web Token.</param>
        /// <returns><see cref="TokenValidationResult"/> of validation.</returns>
        Task<TokenValidationResult> ValidateExpiredTokenAsync(string token);

        /// <summary>
        /// Adds JTI claim of requested JSON Web Token to blacklist (this token will no longer be recognized as valid).
        /// </summary>
        /// <param name="jti">Id of JSON Web Token to be added to blacklist.</param>
        /// <returns><see cref="Task"/> representing <see langword="async"/> operation.</returns>
        Task BlacklistTokenAsync(Guid jti);

        /// <summary>
        /// Verifies against the blacklist if JSON Web Token is blacklisted.
        /// </summary>
        /// <param name="claims">Token claims.</param>
        /// <returns><see langword="true"/> if it is blacklisted, <see langword="false"/> if it is not.</returns>
        Task<bool> IsTokenBlacklistedAsync(List<Claim> claims);
    }
}
