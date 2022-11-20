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
        /// <param name="user"><see cref="User"/> whose <see cref="RefreshToken"/> is to be revoked.</param>
        /// <returns><see cref="Task"/></returns>
        Task RevokeTokenAsync(User user);

        /// <summary>
        /// Replaces old refresh token with a new one.
        /// </summary>
        /// <param name="token">New refresh token.</param>
        /// <returns><see cref="Task"/></returns>
        Task UpdateTokenAsync(RefreshToken token);

        /// <summary>
        /// Validates refresh token.
        /// </summary>
        /// <param name="token">Refresh token to validate.</param>
        /// <param name="user">User to whom refresh token has been issued.</param>
        /// <returns>Returns <see cref="PasswordVerificationResult.Success"/> for valid refresh token, 
        /// <see cref="PasswordVerificationResult.Failed"/> for invalid refresh token.</returns>
        Task<PasswordVerificationResult> ValidateTokenAsync(string token, User user);
    }
}
