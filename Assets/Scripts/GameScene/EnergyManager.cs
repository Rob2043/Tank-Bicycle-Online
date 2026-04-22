using UnityEngine;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using UnityEngine.UI;


public class EnergyManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float maxEnergy = 6000f;
    [SerializeField] private float currentEnergy = 6000f;
    [SerializeField] private float energyConsumption = 200f;
    private EventBus eventBus;
    private MoveSignal moveSignal;
    private Vector2 input;
    public void Init()
    {
        eventBus = ServiceLocator.Current.Get<EventBus>();
        moveSignal = new MoveSignal();
        SimpleEventBus.GetEnergy += GetCurrentEnergy;
        SimpleEventBus.GiveInput += GiveInput;

        if (slider != null)
        {
            slider.maxValue = maxEnergy;
            slider.value = currentEnergy;
        }
    }

    public void Disable()
    {
        SimpleEventBus.GetEnergy -= GetCurrentEnergy;
        SimpleEventBus.GiveInput -= GiveInput;
    }

    private void Update()
    {
        slider.value = currentEnergy;
        eventBus.Invoke(moveSignal);
        if (currentEnergy <= 0f)
        {
            float enrgy = moveSignal.Speed;
            currentEnergy += enrgy * Time.deltaTime;
            return;
        }
        float generatedEnergy = moveSignal.Speed;
        currentEnergy += generatedEnergy * Time.deltaTime;

        float inputPower = Mathf.Clamp01(input.magnitude);
        if (inputPower > 0.01f && currentEnergy > 0f)
        {
            float energyCost = energyConsumption * inputPower * Time.deltaTime;
            currentEnergy -= energyCost;

            if (currentEnergy < 0f)
                currentEnergy = 0f;
        }

        //currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);


        if (slider != null)
            slider.value = currentEnergy;
    }

    private void GiveInput(Vector2 vector)
    {
        input = vector;
    }

    private float GetCurrentEnergy()
    {
        return currentEnergy;
    }
}
