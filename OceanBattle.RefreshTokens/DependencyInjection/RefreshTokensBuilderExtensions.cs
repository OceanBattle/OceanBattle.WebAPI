using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OceanBattle.RefreshTokens.Abstractions;
using OceanBattle.RefreshTokens.DataModel;

namespace OceanBattle.RefreshTokens.DependencyInjection
{
    public static class RefreshTokensBuilderExtensions
    {
        /// <summary>
        /// Adds an Entity Framework implementation of <see cref="RefreshToken"/> stores.
        /// </summary>
        /// <typeparam name="TContext">Entity Framework database context to use.</typeparam>
        /// <param name="builder"><see cref="RefreshTokensBuilder"/> for configuring refresh tokens.</param>
        /// <returns><see cref="RefreshTokensBuilder"/> instance for further configuration.</returns>
        public static RefreshTokensBuilder AddEntityFrameworkStores<TContext>(this RefreshTokensBuilder builder) 
            where TContext : RefreshTokenDbContext
        {
            builder.Services.TryAddScoped<RefreshTokenDbContext>(provider => provider.GetRequiredService<TContext>());
            builder.Services.TryAddTransient<IPasswordHasher<RefreshToken>, PasswordHasher<RefreshToken>>();
            builder.Services.TryAddTransient<IRefreshTokenService, RefreshTokenService>();
            return builder;
        }
    }
}
