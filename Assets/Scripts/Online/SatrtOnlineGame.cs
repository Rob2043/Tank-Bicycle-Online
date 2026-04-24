using UnityEngine;
using Photon;
using Photon.Pun;
using CustomEventBus;

public class SatrtOnlineGame : MonoBehaviour
{
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private ScoreManager scoreManager;

    private void Awake()
    {
        int actorId = PhotonNetwork.LocalPlayer.ActorNumber;
        int spawnIndex = (actorId - 1) % spawnPoints.Length;

        Transform spawn = spawnPoints[spawnIndex];

        GameObject player = PhotonNetwork.Instantiate(
            tankPrefab.name,
            spawn.position,
            spawn.rotation
        );

        scoreManager.scoreText = player.GetComponent<TankOnline>().ScoreText;
    }
}
