using Microsoft.EntityFrameworkCore;
using OceanBattle.RefreshTokens;

namespace OceanBattle.Data
{
    public class AppDbContext : RefreshTokenDbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
