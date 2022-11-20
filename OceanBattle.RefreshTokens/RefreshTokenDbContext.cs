using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OceanBattle.DataModel;
using OceanBattle.RefreshTokens.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .HasOne(rt => rt.User)
                .WithOne()
                .HasForeignKey<RefreshToken>(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.UserId)
                .IsUnique();
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
