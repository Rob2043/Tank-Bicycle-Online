using UnityEngine;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using TMPro;
using System.Collections.Generic;
using TankBycicleOnline.Constants;
using Photon.Pun;


public class ScoreManager : MonoBehaviourPun
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text[] menuScoreTextes = new TMP_Text[5];

    [Header("Points")]
    [SerializeField] private int playerID;

    private Dictionary<int, PlayerData> dataOfPlayers = new Dictionary<int, PlayerData>();
    private List<int> playersId = new List<int>();
    private List<PlayerData> scoreArray = new List<PlayerData>();

    private EventBus eventBus;

    public void Init()
    {
        eventBus = ServiceLocator.Current.Get<EventBus>();
        eventBus.Subscribe<GiveScoreSignal>(GetScore);
        eventBus.Subscribe<GiveScoreSignalOnline>(GiveScoreOnline);


        SimpleEventBus.IsEndGame += EndGameScore;
        SimpleEventBus.GiveTankId += ChangeTankScore;
        SimpleEventBus.SendPlayersID += ChangePlayeID;
    }

    public void Disable()
    {
        if (eventBus != null)
            eventBus.Unsubscribe<GiveScoreSignal>(GetScore);

        SimpleEventBus.IsEndGame -= EndGameScore;
        SimpleEventBus.GiveTankId -= ChangeTankScore;
        SimpleEventBus.SendPlayersID -= ChangePlayeID;
    }

    private void ChangePlayeID(int id)
    {
        playerID = id;
        Debug.Log("Player ID: " + playerID);
    }

    private void ChangeTankScore(ITankId tankId)
    {
        if (tankId == null)
            return;

        AddScore(tankId.ID, -Const.MinusScore, tankId.Name);

        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(
                nameof(RPC_AddScore),
                RpcTarget.Others,
                tankId.ID,
                -Const.MinusScore,
                tankId.Name
            );
        }
    }

    private void GetScore(GiveScoreSignal scoreSignal)
    {
        AddScore(scoreSignal.ID, scoreSignal.Score, scoreSignal.Name);
    }

    [PunRPC]
    private void RPC_AddScore(int id, int score, string playerName)
    {
        AddScore(id, score, playerName);
    }

    private void GiveScoreOnline(GiveScoreSignalOnline scoreOnline)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(
                nameof(RPC_AddScore),
                RpcTarget.Others,
                scoreOnline.ID,
                scoreOnline.Score,
                scoreOnline.Name
            );
        }
    }

    private void AddScore(int id, int score, string playerName)
    {
        if (!dataOfPlayers.ContainsKey(id))
        {
            dataOfPlayers.Add(id, new PlayerData
            {
                Score = score,
                Name = playerName
            });

            playersId.Add(id);
        }
        else
        {
            dataOfPlayers[id].Score += score;

            if (!string.IsNullOrEmpty(playerName))
                dataOfPlayers[id].Name = playerName;
        }

        if (id == playerID && scoreText != null)
        {
            scoreText.text = $"{dataOfPlayers[id].Score}";
        }
    }

    private void EndGameScore()
    {
        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

        scoreArray.Clear();

        foreach (var id in playersId)
        {
            PlayerData data = dataOfPlayers[id];

            int index = scoreArray.FindIndex(x => data.Score > x.Score);

            if (index == -1)
                scoreArray.Add(data);
            else
                scoreArray.Insert(index, data);
        }

        int maxCount = Mathf.Min(scoreArray.Count, menuScoreTextes.Length);

        for (int i = 0; i < maxCount; i++)
        {
            menuScoreTextes[i].text = $"{scoreArray[i].Name}: {scoreArray[i].Score}";
        }

        if (dataOfPlayers.ContainsKey(playerID))
        {
            int bestScore = PlayerPrefs.GetInt("BestScore", 0);
            int currentScore = dataOfPlayers[playerID].Score;

            if (currentScore > bestScore)
            {
                PlayerPrefs.SetInt("BestScore", currentScore);
                PlayerPrefs.Save();
            }
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public int Score;
    public string Name;
}