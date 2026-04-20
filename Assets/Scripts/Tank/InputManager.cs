using System.ComponentModel;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool isKyeBoardInput = false;
    [SerializeField] private Slider  mySlider;
    private ITankInput tankInput;

    public void Initilize()
    {
        if(isKyeBoardInput == true)
        {
            tankInput = new KeyBoardInput();
        }
        else
        {
            tankInput = new SliderInput(mySlider);
        }
    }

    private float GiveSpeed()
    {
        return tankInput.GetSpeed();
    }
}
