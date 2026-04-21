using UnityEngine;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using System.ComponentModel.Design;



public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPositon = new Transform[4];
    private EventBus eventBus;
    

    public void Init()
    {
        eventBus = ServiceLocator.Current.Get<EventBus>();
        eventBus.Subscribe<RespawnSignal>(Respawn);
    }

    public void Disable()
    {
        eventBus.Unsubscribe<RespawnSignal>(Respawn);
    }

    private void Respawn(RespawnSignal respawnSignal)
    {
        int randomPos = UnityEngine.Random.Range(0,spawnPositon.Length);
        respawnSignal.ObjectTransform = spawnPositon[randomPos];
    }
}
