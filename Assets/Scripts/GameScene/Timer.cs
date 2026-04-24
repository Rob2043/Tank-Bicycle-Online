using UnityEngine;
using TMPro;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using Photon.Pun;

public class Timer : MonoBehaviourPun
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private float startTime = 60f;

    private double startNetworkTime;
    private bool isRunning = true;
    private bool isEnded = false;

    private EventBus eventBus;

    public void Init()
    {
        Time.timeScale = 1f;

        eventBus = ServiceLocator.Current.Get<EventBus>();

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startNetworkTime = PhotonNetwork.Time;
                photonView.RPC(nameof(RPC_StartTimer), RpcTarget.AllBuffered, startNetworkTime);
            }
        }
        else
        {
            startNetworkTime = Time.time;
        }
    }

    private void Update()
    {
        if (!isRunning)
            return;

        double currentTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.time;
        float elapsed = (float)(currentTime - startNetworkTime);
        float timeLeft = startTime - elapsed;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            isRunning = false;

            if (!isEnded)
            {
                isEnded = true;
                TimerEnd();
            }
        }

        UpdateTimerText(timeLeft);
    }

    [PunRPC]
    private void RPC_StartTimer(double networkStartTime)
    {
        startNetworkTime = networkStartTime;
        isRunning = true;
        isEnded = false;
    }

    private void UpdateTimerText(float time)
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

        SimpleEventBus.IsEndGame?.Invoke();

        Debug.Log("Timer finished!");
    }
}