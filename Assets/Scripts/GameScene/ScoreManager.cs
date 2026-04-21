using UnityEngine;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text[] menuScoreTextes = new TMP_Text[5];
    [Header("Points")]
    [SerializeField] private int playerID;
    private Dictionary<int, int> dataOfPlayers = new Dictionary<int, int>();
    private List<int> playersId = new List<int>();
    private List<int> scoreArray = new List<int>();
    private int playersScore = 0;
    private EventBus eventBus;
    private GiveScoreSignal giveScoreSignal;

    public void Init()
    {
        eventBus = ServiceLocator.Current.Get<EventBus>();
        eventBus.Subscribe<GiveScoreSignal>(GetScore);
        SimpleEventBus.IsEndGame += EndGameScore;
    }

    public void Disable()
    {
        eventBus.Unsubscribe<GiveScoreSignal>(GetScore);
        SimpleEventBus.IsEndGame -= EndGameScore;
    }

    private void GetScore(GiveScoreSignal scoreSignal)
    {
        if (!dataOfPlayers.ContainsKey(scoreSignal.Name))
        {
            dataOfPlayers.Add(scoreSignal.Name, scoreSignal.Score);
            playersId.Add(scoreSignal.Name);
        }
        else
        {
            dataOfPlayers[scoreSignal.Name] += scoreSignal.Score;
        }

        if (scoreSignal.Name == playerID)
        {
            scoreText.text = $"{dataOfPlayers[playerID]}";
        }

    }

    private void EndGameScore()
    {
        scoreText.gameObject.SetActive(false);
        //menuScoreText.text = $"{dataOfPlayers[playerID]}";
        foreach (var item in playersId)
        {
            int score = dataOfPlayers[item];
            int index = scoreArray.FindIndex(x => score > x);

            if(index == -1)
                scoreArray.Add(score);
            else
                scoreArray.Insert(index,score);
        }
        for (int i = 0; i < menuScoreTextes.Length; i++)
        {
            menuScoreTextes[i].text = $"{scoreArray[i]}";
        }

    }

}
