
using System.Diagnostics.Contracts;
using UnityEngine;

namespace TankBycicleOnline.CallBacks
{
    public class GiveScoreSignalOnline
    {
        public int Score;
        public int ID;
        public string Name;
        public GiveScoreSignalOnline(int _score, int _id, string name)
        {
            Score = _score;
            ID = _id;
            Name = name;
        }
    }
}
