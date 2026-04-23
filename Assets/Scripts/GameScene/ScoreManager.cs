using UnityEngine;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using TMPro;
using System.Collections.Generic;
using TankBycicleOnline.Constants;


public class ScoreManager : MonoBehaviour
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
        SimpleEventBus.IsEndGame += EndGameScore;
        SimpleEventBus.GiveTankId += ChangeTankScore;
    }

    public void Disable()
    {
        eventBus.Unsubscribe<GiveScoreSignal>(GetScore);
        SimpleEventBus.IsEndGame -= EndGameScore;
        SimpleEventBus.GiveTankId -= ChangeTankScore;
    }
    
    private void ChangeTankScore(ITankId tankId)
    {
        dataOfPlayers[tankId.ID].Score -= Const.MinusScore;
    }

    private void GetScore(GiveScoreSignal scoreSignal)
    {
        if (!dataOfPlayers.ContainsKey(scoreSignal.ID))
        {
            dataOfPlayers.Add(scoreSignal.ID, new PlayerData{ Score = scoreSignal.Score, Name = scoreSignal.Name});
            playersId.Add(scoreSignal.ID);
        }
        else
        {
            dataOfPlayers[scoreSignal.ID].Score += scoreSignal.Score;
        }

        if (scoreSignal.ID == playerID)
        {
            scoreText.text = $"{dataOfPlayers[playerID].Score}";
        }

    }

    private void EndGameScore()
    {
        scoreText.gameObject.SetActive(false);
        //menuScoreText.text = $"{dataOfPlayers[playerID]}";
        foreach (var item in playersId)
        {
            PlayerData data = dataOfPlayers[item];
            int index = scoreArray.FindIndex(x => data.Score > x.Score);

            if(index == -1)
                scoreArray.Add(data);
            else
                scoreArray.Insert(index,data);
        }
        for (int i = 0; i < scoreArray.Count; i++)
        {
            menuScoreTextes[i].text = $"{scoreArray[i].Name}: {scoreArray[i].Score}";
        }
        int bestScore = PlayerPrefs.GetInt("BestScore",0);
        int bestPlayerScore = bestScore < dataOfPlayers[playerID].Score ?  dataOfPlayers[playerID].Score : bestScore;
        PlayerPrefs.SetInt("BestScore",bestPlayerScore);
        PlayerPrefs.Save(); 

    }

}

[System.Serializable]
public class PlayerData
{
    public int Score;
    public string Name;
}