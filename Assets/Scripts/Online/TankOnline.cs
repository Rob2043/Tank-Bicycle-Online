using UnityEngine;
using Photon;
using Photon.Pun;
using Tanks.Complete;
using UnityEngine.UI;

public class TankOnline : MonoBehaviourPun
{
    [SerializeField] private Slider HealthPlayer;
    [SerializeField] private Slider HealthUI;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _playersUI;
    [SerializeField] private InputManager inputManager;
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
            _camera.SetActive(false);
            inputManager.enabled = false;
        }
    }
}
