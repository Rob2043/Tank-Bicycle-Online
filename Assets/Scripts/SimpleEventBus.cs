using UnityEngine;
using System;

namespace CustomEventBus
{
    public class SimpleEventBus
    {
        public static Action IsEndGame;
        public static Func<float> GetEnergy;
        public static Action<Vector2> GiveInput;
        public static Action<ITankId> GiveTankId;
        #region Online
        public static Action OpenLobby; 
        public static Action CloseLobby; 
        public static Action<int> SendPlayersID;
        public static Action AddScoreOnline;
        #endregion Online
    }
}

