
using System.Diagnostics.Contracts;
using UnityEngine;

namespace TankBycicleOnline.CallBacks
{
    public class GiveScoreSignal
    {
        public int Score;
        public int ID;
        public string Name;
        public GiveScoreSignal(int _score, int _id, string name)
        {
            Score = _score;
            ID = _id;
            Name = name;
        }
    }
}
