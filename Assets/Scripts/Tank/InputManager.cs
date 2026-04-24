using UnityEngine;
using UnityEngine.UI;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using Photon.Pun;
using Tanks.Complete;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool isKyeBoardInput = false;
    [SerializeField] private Slider  mySlider;
    private ITankInput tankInput;
    private EventBus _eventBus;

    private PhotonView  photonView;

    public void Init()
    {
        _eventBus = ServiceLocator.Current.Get<EventBus>();
        _eventBus.Subscribe<MoveSignal>(GiveSpeed);
        if(isKyeBoardInput == true)
        {
            tankInput = new KeyBoardInput();
        }
        else
        {
            tankInput = new SliderInput(mySlider);
        }

    }

    private void Start() {
        photonView = GetComponent<PhotonView>();
        if(PhotonNetwork.InRoom && photonView.IsMine)
        {
            Init();
        }
    }

    private void GiveSpeed(MoveSignal moveSignal)
    {
        moveSignal.Speed = tankInput.GetSpeed();
    }

    public void Disable()
    {
        _eventBus.Unsubscribe<MoveSignal>(GiveSpeed);
    }
}
