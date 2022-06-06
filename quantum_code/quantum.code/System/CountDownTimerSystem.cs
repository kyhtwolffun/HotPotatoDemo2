using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class CountDownTimerSystem : SystemMainThreadFilter<CountDownTimerSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef entity;
            public CountDownTimerComp* countDownTimerComp;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.countDownTimerComp->timer > 0)
                filter.countDownTimerComp->timer -= f.DeltaTime;
            else
                filter.countDownTimerComp->timeSup = true;
        }
    }
}
