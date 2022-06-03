using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Deterministic;

namespace Quantum
{
    class CountDown : SystemMainThread
    {
        private FP time;
        private FP timer;
        public bool TimeSup => timer <= 0;

        public CountDown(FP _time)
        {
            time = _time;
            timer = time;
        }

        public override void Update(Frame f)
        {
            if (!TimeSup)
            {
                time -= f.DeltaTime;
            }
        }

        public void Reset()
        {
            timer = time;
        }
    }
}
