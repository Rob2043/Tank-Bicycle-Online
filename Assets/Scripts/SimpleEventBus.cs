using UnityEngine;
using System;


namespace CustomEventBus
{
    public class SimpleEventBus
    {
        public static Action IsEndGame;
        public static Func<float> GetEnergy;
        public static Action<Vector2> GiveInput;
    }
}

