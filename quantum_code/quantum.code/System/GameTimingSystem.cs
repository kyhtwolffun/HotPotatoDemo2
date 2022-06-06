using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class GameTimingSystem : SystemMainThreadFilter<GameTimingSystem.Filter>
    {
        public override void OnInit(Frame f)
        {
            base.OnInit(f);

            //InGame PhaseTimer
            var phase = f.Unsafe.GetOrAddSingletonPointer<GamePhaseComp>();
            phase->gamePhase = GamePhase.startGame;
            phase->currentRound = 0;

            var gameConfigRefComp = f.GetSingleton<GameConfigRefComp>();
            var gameConfigRefAsset = f.FindAsset<GameParameter>(gameConfigRefComp.gameParameter.Id);

            if (f.Unsafe.TryGetPointer<CountDownTimerComp>(f.GetSingletonEntityRef<GamePhaseComp>(), out var timer))
            {
                timer->timer = gameConfigRefAsset.gameStartDelay;
                timer->timeSup = false;
            }
            else
            {
                f.Add<CountDownTimerComp>(f.GetSingletonEntityRef<GamePhaseComp>());
                var newTimer = f.Unsafe.GetPointer<CountDownTimerComp>(f.GetSingletonEntityRef<GamePhaseComp>());
                newTimer->timer = gameConfigRefAsset.gameStartDelay;
                newTimer->timeSup = false;
            }
        }

        public struct Filter
        {
            public EntityRef entity;
            public GamePhaseComp* gamePhaseComp;
            public CountDownTimerComp* countDownTimerComp;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var gameConfigRefComp = f.GetSingleton<GameConfigRefComp>();
            var gameConfigRefAsset = f.FindAsset<GameParameter>(gameConfigRefComp.gameParameter.Id);

            if (filter.countDownTimerComp->timeSup && filter.gamePhaseComp->currentRound < gameConfigRefAsset.roundCount)
            {
                switch (filter.gamePhaseComp->gamePhase)
                {
                    case GamePhase.spreadBomb:
                        f.Signals.RandomBombForPlayers();
                        filter.gamePhaseComp->currentRound++;
                        filter.countDownTimerComp->timer = gameConfigRefAsset.bombExplodeTime + gameConfigRefAsset.restTimeBetweenEachRound;
                        filter.countDownTimerComp->timeSup = false;
                        break;
                    default:
                        filter.gamePhaseComp->gamePhase = GamePhase.spreadBomb;
                        break;
                }
            }
        }

        //public override void Update(Frame f)
        //{
        //    var gameConfigRefComp = f.GetSingleton<GameConfigRefComp>();
        //    var gameConfigRefAsset = f.FindAsset<GameParameter>(gameConfigRefComp.gameParameter.Id);

        //    var phaseTimer = f.Filter<GamePhaseComp, CountDownTimerComp>();

        //    while (phaseTimer.NextUnsafe(out var entity, out var gamePhaseComp, out var countDownTimerComp))
        //    {
        //        if (countDownTimerComp->timeSup && gamePhaseComp->currentRound < gameConfigRefAsset.roundCount)
        //        {
        //            switch (gamePhaseComp->gamePhase)
        //            {
        //                case GamePhase.spreadBomb:
        //                    f.Signals.RandomBombForPlayers();
        //                    gamePhaseComp->currentRound++;
        //                    countDownTimerComp->timer = gameConfigRefAsset.bombExplodeTime + gameConfigRefAsset.restTimeBetweenEachRound;
        //                    countDownTimerComp->timeSup = false;
        //                    break;
        //                default:
        //                    gamePhaseComp->gamePhase = GamePhase.spreadBomb;
        //                    break;
        //            }
        //        }
        //    }


        //    //var gameTimer = f.Unsafe.GetPointerSingleton<CountDownTimerComp>();

        //    //if (gameTimer->timeSup)
        //    //{
        //    //    switch (gameTimer->gamePhase)
        //    //    {
        //    //        case GamePhase.spreadBomb:
        //    //            f.Signals.RandomBombForPlayers();
        //    //            gameTimer->timer = gameConfigRefAsset.bombExplodeTime + gameConfigRefAsset.restTimeBetweenEachRound;
        //    //            gameTimer->timeSup = false;
        //    //            break;
        //    //        default:
        //    //            gameTimer->gamePhase = GamePhase.spreadBomb;
        //    //            break;
        //    //    }
        //    //}
        //}
    }
}
