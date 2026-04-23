using UnityEngine;
using Photon;
using Photon.Pun;
using Tanks.Complete;
using UnityEngine.UI;

public class TankOnline : MonoBehaviourPun
{
    [SerializeField] private Slider HealthPlayer;
    [SerializeField] private Image FillImagePlayer;
    [SerializeField] private Slider HealthUI;
    [SerializeField] private Image FillImageUI;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _playersUI;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private EnergyManager energyManager;
    private TankHealth tankHealth;
    private void Awake()
    {
        tankHealth = GetComponent<TankHealth>();
        if (photonView.IsMine)
        {
            tankHealth.m_Slider = HealthUI;
            tankHealth.m_FillImage = FillImageUI;
            HealthPlayer.gameObject.SetActive(false);
            inputManager.enabled = true;
            energyManager.enabled = true;
        }
        else
        {
            tankHealth.m_Slider = HealthPlayer;
            tankHealth.m_FillImage = FillImagePlayer;
            HealthPlayer.gameObject.SetActive(true);
            _playersUI.SetActive(false);
            _camera.SetActive(false);
            inputManager.enabled = false;
            energyManager.enabled = false;
        }
    }
}
