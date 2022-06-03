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
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.bombMark->timer > 0)
            {
                filter.bombMark->timer -= f.DeltaTime;
            }
            else if (!filter.bombMark->bombMark)
            {
                Log.Info("Explode");
                filter.bombMark->bombMark = true;
                f.Events.Explode();
                f.Destroy(filter.Entity);
            }
        }

        public void OnCollisionEnter3D(Frame f, CollisionInfo3D info)
        {
            var set = ComponentSet.Create<BombMarkComp, MovementComp>();

            if (f.Has<BombMarkComp>(info.Entity) && !f.Has<BombMarkComp>(info.Other) && f.Has<MovementComp>(info.Other))
            {
                f.Add<BombMarkComp>(info.Other);
                f.Unsafe.TryGetPointer<BombMarkComp>(info.Other, out var bombMarkOther);
                f.Unsafe.TryGetPointer<BombMarkComp>(info.Other, out var bombMarkEnity);

                bombMarkOther->timer = bombMarkEnity->timer;

                f.Remove<BombMarkComp>(info.Entity);
            }
        }
    }
}
