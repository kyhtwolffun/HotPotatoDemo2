using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class PlayerDataSetSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            var data = f.GetPlayerData(player);
            var prototype = f.FindAsset<EntityPrototype>(data.characterPrototype.Id);
            var e = f.Create(prototype);

            if (f.Unsafe.TryGetPointer<PlayerLink>(e, out var pl))
            {
                pl->Player = player;
            }

            if (f.Unsafe.TryGetPointer<Transform3D>(e, out var t))
            {
                t->Position.X = player._index;
            }
        }
    }
}
