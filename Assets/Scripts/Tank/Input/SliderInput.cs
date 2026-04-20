using UnityEngine;
using UnityEngine.UI;


public class SliderInput : ITankInput
{
    private Slider slider;


    public SliderInput(Slider slideInput)
    {
        this.slider = slideInput;
    }

    public float GetSpeed()
    {
        return slider.value;
    }

    public float GetTurn()
    {
        return 0f;
    }
}
