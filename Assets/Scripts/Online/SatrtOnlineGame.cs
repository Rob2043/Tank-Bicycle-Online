using UnityEngine;
using Photon;
using Photon.Pun;

public class SatrtOnlineGame : MonoBehaviour
{
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        PhotonNetwork.Instantiate(
            tankPrefab.name,
            spawn.position,
            spawn.rotation
        );
    }
}
