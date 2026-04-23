using UnityEngine;
using Photon;
using Photon.Pun;
using TankBycicleOnline.Constants;

public class SatrtOnlineGame : MonoBehaviour
{
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;

        Transform spawn = spawnPoints[index];

        PhotonNetwork.Instantiate(
            tankPrefab.name,
            spawn.position,
            spawn.rotation
        );

    }
}
