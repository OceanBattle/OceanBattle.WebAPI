using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OceanBattle.RefreshTokens.DataModel;
using OceanBattle.DataModel;

namespace OceanBattle.RefreshTokens
{
    public abstract class RefreshTokenDbContext : IdentityDbContext<User>
    {
        public RefreshTokenDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Jti)
                .IsUnique();

            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.UserId);
        }

        /// <summary>
        /// Database table containing <see cref="RefreshToken"/> tokens.
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
