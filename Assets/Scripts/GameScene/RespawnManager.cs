using UnityEngine;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using Photon.Pun;


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
        PhotonView view = respawnSignal.ObjectTransform.GetComponent<PhotonView>();

        if (PhotonNetwork.InRoom && (view == null || !view.IsMine))
            return;

        int randomPos = Random.Range(0, spawnPositon.Length);
        Transform spawn = spawnPositon[randomPos];

        // Локально перемещаем
        respawnSignal.ObjectTransform.position = spawn.position;
        respawnSignal.ObjectTransform.rotation = spawn.rotation;

        if (PhotonNetwork.InRoom)
        {
            view.RPC("RPC_Respawn",RpcTarget.Others,spawn.position,spawn.rotation);
        }

        SimpleEventBus.GiveTankId?.Invoke(respawnSignal.TankId);
    }
}
