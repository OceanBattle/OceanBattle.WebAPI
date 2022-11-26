using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OceanBattle.DataModel;
using OceanBattle.RefreshTokens.Abstractions;
using OceanBattle.RefreshTokens.DataModel;

namespace OceanBattle.RefreshTokens
{
    /// <summary>
    /// Managing refresh tokens.
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly RefreshTokenDbContext _dbContext;
        private readonly IPasswordHasher<RefreshToken> _passwordHasher;

        public RefreshTokenService(
            RefreshTokenDbContext dbContext,
            IPasswordHasher<RefreshToken> passwordHasher) 
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Revokes issued refresh token (token becomes invalid for future validation attempts).
        /// </summary>
        /// <param name="jti">ID of JSON Web Token corresponding to refresh token that is to be removed.</param>
        /// <returns><see cref="Task"/> of <see langword="async"/> operation.</returns>
        public async Task RevokeTokenAsync(Guid jti)
        {
            _dbContext.RefreshTokens.RemoveRange(
                _dbContext.RefreshTokens.Where(rt => rt.Jti == jti));

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Replaces old refresh token with a new one.
        /// </summary>
        /// <param name="token">New refresh token.</param>
        /// <returns><see cref="Task"/></returns>
        public async Task AddTokenAsync(RefreshToken token)
        {
            RefreshToken newToken = new RefreshToken
            {
                Token = _passwordHasher.HashPassword(token, token.Token!),
                ExpirationDate = token.ExpirationDate,
                Jti = token.Jti,
                UserId = token.UserId
            };

            _dbContext.RefreshTokens.Add(newToken);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Validates refresh token.
        /// </summary>
        /// <param name="token">Refresh token to validate.</param>
        /// <param name="jti">ID of JSON Web Token assigned to this refresh token.</param>
        /// <returns>Returns <see cref="PasswordVerificationResult.Success"/> for valid refresh token, 
        /// <see cref="PasswordVerificationResult.Failed"/> for invalid refresh token.</returns>
        public async Task<PasswordVerificationResult> ValidateTokenAsync(string token, Guid jti)
        {
            RefreshToken? refreshToken = 
                await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Jti == jti);
            
            if (refreshToken is null || 
                refreshToken.Token is null ||
                refreshToken.ExpirationDate < DateTime.Now)
                return PasswordVerificationResult.Failed;

            return _passwordHasher.VerifyHashedPassword(refreshToken, refreshToken.Token!, token);
        }

        /// <summary>
        /// Revokes all <see cref="RefreshToken"/> tokens assigned to <see cref="User"/>
        /// </summary>
        /// <param name="userId">ID of user whose tokens will be revoked.</param>
        /// <returns><see cref="Task"/> of <see langword="async"/> operation.</returns>
        public async Task RevokeTokensAsync(string userId)
        {
            _dbContext.RefreshTokens.RemoveRange(
                _dbContext.RefreshTokens.Where(rt => rt.UserId == userId));

            await _dbContext.SaveChangesAsync();
        }
    }
}
