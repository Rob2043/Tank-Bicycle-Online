
using System.Diagnostics.Contracts;
using UnityEngine;

namespace TankBycicleOnline.CallBacks
{
    public class GiveScoreSignal
    {
        public int Score;
        public int Name;
        public GiveScoreSignal(int _score, int name)
        {
            Score = _score;
            Name = name;
        }
    }
}
