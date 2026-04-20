using UnityEngine;
using UnityEngine.UI;
using CustomEventBus;
using TankBycicleOnline.CallBacks;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool isKyeBoardInput = false;
    [SerializeField] private Slider  mySlider;
    private ITankInput tankInput;
    private EventBus _eventBus;

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

    private void GiveSpeed(MoveSignal moveSignal)
    {
        moveSignal._speed = tankInput.GetSpeed();
    }
}
