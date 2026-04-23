using UnityEngine;
using Photon;
using Photon.Pun;
using Tanks.Complete;
using UnityEngine.UI;

public class TankOnline : MonoBehaviourPun
{
    [SerializeField] private Slider HealthPlayer;
    [SerializeField] private Slider HealthUI;
    private TankHealth tankHealth;
    private void Start()
    {
        tankHealth = GetComponent<TankHealth>();
        if (photonView.IsMine)
        {
            tankHealth.m_Slider = HealthUI;
            HealthPlayer.gameObject.SetActive(false);
        }
        else
        {
            tankHealth.m_Slider = HealthPlayer;
            HealthPlayer.gameObject.SetActive(false);
        }
    }
}
