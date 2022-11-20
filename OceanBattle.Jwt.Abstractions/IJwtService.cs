using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    }
}
