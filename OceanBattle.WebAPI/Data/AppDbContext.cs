using Microsoft.EntityFrameworkCore;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.DataModel.Game.Ships;
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

            builder.Entity<Ship>()
                   .HasOne(s => s.User)
                   .WithMany(u => u.OwnedVessels);

            builder.Entity<Ship>()
                   .Ignore(s => s.Cells)
                   .Ignore(s => s.Orientation);

            builder.Entity<Warship>()
                .HasMany(w => w.Weapons);
        }

        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Warship> Warships { get; set; }
        public DbSet<Corvette> Corvettes { get; set; }
        public DbSet<Frigate> Frigates { get; set; }
        public DbSet<Destroyer> Destroyers { get; set; }
        public DbSet<Cruiser> Cruisers { get; set; }
        public DbSet<Battleship> Battleships { get; set; }
    }
}
