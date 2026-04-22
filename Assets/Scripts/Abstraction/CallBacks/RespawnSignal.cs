using UnityEngine;

namespace TankBycicleOnline.CallBacks
{
    public class RespawnSignal
    {
        public ITankId TankId;
        public Transform ObjectTransform;

        public RespawnSignal(Transform _transform,ITankId tankId)
        {
            ObjectTransform = _transform;
            TankId = tankId;
        }
    }   
}

