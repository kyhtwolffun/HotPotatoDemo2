using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class GameTimingSystem : SystemMainThread
    {
        public override bool StartEnabled => false;
        private FP timer;

        public override void OnInit(Frame f)
        {
            base.OnInit(f);
            timer = 0;
        }

        public override void Update(Frame f)
        {
            Log.Info("ALOOO_" + timer);
            var gameConfigRefComp = f.GetSingleton<GameConfigRefComp>();
            var gameConfigRefAsset = f.FindAsset<GameParameter>(gameConfigRefComp.gameParameter.Id);

            if (timer < gameConfigRefAsset.matchTime && 
                timer % gameConfigRefAsset.bombExplodeTime < f.DeltaTime)
            {
                f.Signals.RandomBombForPlayers();
                Log.Info("Bomb: Spread BOMBBBBB");
                f.SystemDisable<GameTimingSystem>();
            }
            timer += f.DeltaTime;
        }
    }
}
