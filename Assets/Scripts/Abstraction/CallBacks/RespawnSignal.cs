using UnityEngine;

namespace TankBycicleOnline.CallBacks
{
    public class RespawnSignal
    {
        public Transform ObjectTransform;

        public RespawnSignal(Transform _transform)
        {
            ObjectTransform = _transform;
        }
    }   
}

