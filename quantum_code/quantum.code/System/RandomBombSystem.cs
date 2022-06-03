using Quantum;
using Photon;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class RandomBombSystem : SystemSignalsOnly, ISignalRandomBombForPlayers
    {
        public void RandomBombForPlayers(Frame f)
        {
            var gameConfigRefComp = f.GetSingleton<GameConfigRefComp>();
            var gameConfigRefAsset = f.FindAsset<GameParameter>(gameConfigRefComp.gameParameter.Id);

            var playerCount = f.ComponentCount<PlayerLink>();
            var bombCount = f.ComponentCount<BombMarkComp>();

            for (int i = 0; i < gameConfigRefAsset.bombCount.Count; i++)
            {
                if (playerCount >= gameConfigRefAsset.bombCount[i].minPlayer)
                {
                    if (bombCount > 0)
                        EraseAllBomb(f);

                    RandomBomb(f, gameConfigRefAsset.bombCount[i].bomb, gameConfigRefAsset.bombExplodeTime, playerCount);

                    break;
                }
            }
        }

        private void RandomBomb(Frame f, int bomb, FP bombTimer, int player)
        {
            if (bomb <= 0 || player <= 0)
                return;

            var rand = f.RNG;
            for (int i = 0; i < bomb; i++)
            {
                var players = f.Filter<PlayerLink>();

                int playerIndex = 0;

                //TODO: Need optimizing
                randomInt:
                var randInt = rand->NextInclusive(1, player);

                while (players.NextUnsafe(out var e, out var pl))
                {
                    playerIndex++;
                    if (playerIndex == randInt)
                    {
                        if (f.Has<BombMarkComp>(e))
                        {
                            goto randomInt;
                        }
                        else
                        {
                            f.Add<BombMarkComp>(e);
                            f.Unsafe.TryGetPointer<BombMarkComp>(e, out var bombMark);
                            bombMark->timer = bombTimer;
                            break;
                        }
                    }
                }

            }
        }

        private void EraseAllBomb(Frame f)
        {
            var players = f.Filter<PlayerLink, BombMarkComp>();
            while (players.NextUnsafe(out var e, out var pl, out var bm))
            {
                f.Remove<BombMarkComp>(e);
            }
        }      
    }
}
