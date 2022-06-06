using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class BombMarkSystem : SystemMainThreadFilter<BombMarkSystem.Filter>, ISignalOnCollisionEnter3D
    {
        public struct Filter
        {
            public EntityRef Entity;
            public BombMarkComp* bombMark;
            public PlayerLink* link;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.bombMark->timer > 0)
            {
                Log.Info("Bomb: countdown");
                filter.bombMark->timer -= f.DeltaTime;
                f.Events.BombMark(filter.link->Player, true, filter.bombMark->timer);
            }
            else if (!filter.bombMark->isExploded)
            {
                Log.Info("Bomb: Explode");
                filter.bombMark->isExploded = true;
                f.Events.Explode(filter.link->Player);
                f.Destroy(filter.Entity);
            }
        }

        public void OnCollisionEnter3D(Frame f, CollisionInfo3D info)
        {
            if (f.Has<BombMarkComp>(info.Entity) && !f.Has<BombMarkComp>(info.Other) && f.Has<PlayerLink>(info.Other))
            {
                f.Add<BombMarkComp>(info.Other);
                if (f.Unsafe.TryGetPointer<BombMarkComp>(info.Other, out var bombMarkOther) &&
                    f.Unsafe.TryGetPointer<BombMarkComp>(info.Entity, out var bombMarkEnity))
                {
                    bombMarkOther->timer = bombMarkEnity->timer;
                    f.Remove<BombMarkComp>(info.Entity);
                    f.Events.BombMark(f.Unsafe.GetPointer<PlayerLink>(info.Entity)->Player, false, 0);
                }

            }
        }
    }
}
