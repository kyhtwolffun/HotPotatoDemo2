using Quantum;
using Photon;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class RandomBombSystem : SystemMainThreadFilter<RandomBombSystem.Filter>, ISignalOnPlayerDataSet
    {
        public struct Filter
        {
            public EntityRef entity;
            public PlayerLink* link;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            //throw new NotImplementedException();
        }

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

            //var gameConfig = f.Filter<GameConfigRefComp>();
            var gameConfigRefComp = f.GetSingleton<GameConfigRefComp>();
            var gameConfigRefAsset = f.FindAsset<GameParameter>(gameConfigRefComp.gameParameter.Id);

            //var players = f.GetComponentIterator<PlayerLink>();
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
