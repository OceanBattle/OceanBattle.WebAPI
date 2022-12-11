using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.DataModel.Game.Ships;
using OceanBattle.Game.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Repositories
{
    public class LevelsRepository : ILevelsRepository
    {
        public IEnumerable<Level> GetLevels()
        {
            return new Level[]
            {
                new Level
                {
                    BattlefieldSize = 8,
                    AvailableTypes = new (Type type, int maxAmount)[]
                    {
                        (typeof(Corvette), 4),
                        (typeof(Frigate), 1)
                    }
                },

                new Level
                {
                    BattlefieldSize = 10,
                    AvailableTypes = new (Type type, int maxAmount)[]
                    {
                        (typeof(Corvette), 4),
                        (typeof(Frigate), 2)
                    }
                },

                new Level
                {
                    BattlefieldSize = 12,
                    AvailableTypes = new (Type type, int maxAmount)[]
                    {
                        (typeof(Corvette), 4),
                        (typeof(Frigate), 2),
                        (typeof(Destroyer), 1)
                    }
                }
            };
        }
    }
}
