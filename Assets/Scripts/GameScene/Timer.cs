using UnityEngine;
using TMPro;
using CustomEventBus;
using TankBycicleOnline.CallBacks;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private float startTime = 60f;

    private float time;
    private bool isRunning = true;

    private EventBus eventBus;

    public void Init()
    {
        Time.timeScale = 1f;
        eventBus = ServiceLocator.Current.Get<EventBus>();
        time = startTime;
    }

    private void Update()
    {
        if (!isRunning) return;

        time -= Time.deltaTime;

        if (time <= 0)
        {
            time = 0;
            isRunning = false;
            TimerEnd();
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void TimerEnd()
    {
        Time.timeScale = 0f;
        endPanel.SetActive(true);
        gamePanel.SetActive(false);
        SimpleEventBus.IsEndGame.Invoke();
        Debug.Log("Timer finished!");
    }
}