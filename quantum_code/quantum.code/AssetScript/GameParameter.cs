using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Deterministic;

namespace Quantum
{
    public partial class GameParameter
    {
        public FP gameStartDelay;
        public FP restTimeBetweenEachRound;

        public FP bombExplodeTime;
        public FP matchTime;
        public List<BombCount> bombCount;
    }

}

[Serializable]
public struct BombCount
{
    public int minPlayer;
    public int bomb;
}