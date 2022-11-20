using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OceanBattle.DataModel;
using OceanBattle.RefreshTokens.Abstractions;
using OceanBattle.RefreshTokens.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.RefreshTokens
{
    /// <summary>
    /// Managing refresh tokens.
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly RefreshTokenDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public RefreshTokenService(
            RefreshTokenDbContext dbContext,
            IPasswordHasher<User> passwordHasher) 
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Revokes issued refresh token (token becomes invalid for future validation attempts).
        /// </summary>
        /// <param name="user"><see cref="User"/> whose <see cref="RefreshToken"/> is to be revoked.</param> 
        public async Task RevokeTokenAsync(User user)
        {
            RefreshToken? token = 
                await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == user.Id);

            if (token is null)
                return;

            _dbContext.RefreshTokens.Remove(token);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Replaces old refresh token with a new one.
        /// </summary>
        /// <param name="token">New refresh token.</param>
        /// <returns><see cref="Task"/></returns>
        public async Task UpdateTokenAsync(RefreshToken token)
        {
            RefreshToken newToken = new RefreshToken
            {
                Token = _passwordHasher.HashPassword(token.User!, token.Token!),
                ExpirationDate = token.ExpirationDate,
                UserId = token.UserId,
                User = token.User
            };

            RefreshToken? oldToken = 
                await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == token.UserId);

            if (oldToken is null)
                _dbContext.RefreshTokens.Add(newToken);
            else
            {
                oldToken.Token = newToken.Token;
                oldToken.ExpirationDate = newToken.ExpirationDate;
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Validates refresh token.
        /// </summary>
        /// <param name="token">Refresh token to validate.</param>
        /// <param name="user">User to whom refresh token has been issued.</param>
        /// <returns>Returns <see cref="PasswordVerificationResult.Success"/> for valid refresh token, 
        /// <see cref="PasswordVerificationResult.Failed"/> for invalid refresh token.</returns>
        public async Task<PasswordVerificationResult> ValidateTokenAsync(string token, User user)
        {
            RefreshToken? refreshToken = 
                await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == user.Id);

            if (refreshToken is null || 
                refreshToken.Token is null ||
                refreshToken.ExpirationDate < DateTime.Now)
                return PasswordVerificationResult.Failed;

            return _passwordHasher.VerifyHashedPassword(user, refreshToken.Token!, token);
        }
    }
}
