using Microsoft.AspNetCore.Identity;
using OceanBattle.DataModel;
using OceanBattle.RefreshTokens.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.RefreshTokens.Abstractions
{
    /// <summary>
    /// Managing refresh tokens.
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Revokes issued refresh token (token becomes invalid for future validation attempts).
        /// </summary>
        /// <param name="jti">ID of JSON Web Token corresponding to refresh token that is to be removed.</param>
        /// <returns><see cref="Task"/> of <see langword="async"/> operation.</returns>>
        Task RevokeTokenAsync(Guid jti);

        /// <summary>
        /// Adds new refresh token.
        /// </summary>
        /// <param name="token">New refresh token.</param>
        /// <returns><see cref="Task"/></returns>
        Task AddTokenAsync(RefreshToken token);

        /// <summary>
        /// Validates refresh token.
        /// </summary>
        /// <param name="token">Refresh token to validate.</param>
        /// <param name="jti">ID of JSON Web Token assigned to this refresh token.</param>
        /// <returns>Returns <see cref="PasswordVerificationResult.Success"/> for valid refresh token, 
        /// <see cref="PasswordVerificationResult.Failed"/> for invalid refresh token.</returns>
        Task<PasswordVerificationResult> ValidateTokenAsync(string token, Guid jti);

        /// <summary>
        /// Revokes all <see cref="RefreshToken"/> tokens assigned to <see cref="User"/>
        /// </summary>
        /// <param name="userId">ID of user whose tokens will be revoked.</param>
        /// <returns><see cref="Task"/> of <see langword="async"/> operation.</returns>
        Task RevokeTokensAsync(string userId);
    }
}
