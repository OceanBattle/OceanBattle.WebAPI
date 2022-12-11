using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.DataModel.Game.Ships;
using OceanBattle.Game.Abstractions;

namespace OceanBattle.Game.Repositories
{
    public class ShipsRepository : IShipsRepository
    {
        public IEnumerable<Ship> GetShips()
        {
            return new Ship[] 
            {
                new Corvette(100)
                {
                    Weapons = new[]
                    {
                        new Weapon { Damage = 20, DamageRadius = 1 }
                    }
                },

                new Frigate(150)
                {
                    Weapons = new[]
                    {
                        new Weapon { Damage = 50, DamageRadius = 1 }
                    }
                },

                new Destroyer(150) 
                {
                    Weapons = new[]
                    {
                        new Weapon { Damage = 50, DamageRadius = 2 }
                    }
                },

                new Cruiser(200)
                {
                    Weapons = new[]
                    {
                        new Weapon { Damage = 150, DamageRadius = 2 },
                        new Weapon { Damage = 150, DamageRadius = 2 }
                    }
                },

                new Battleship(250)
                {
                    Weapons = new[]
                    {
                        new Weapon { Damage = 100, DamageRadius = 3 },
                        new Weapon { Damage = 100, DamageRadius = 3 }
                    }
                }
            };
        }
    }
}
