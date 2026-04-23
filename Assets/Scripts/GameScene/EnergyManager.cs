using UnityEngine;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using UnityEngine.UI;
using TankBycicleOnline.Constants;


public class EnergyManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float maxEnergy = 6000f;
    [SerializeField] private float currentEnergy = 6000f;
    [SerializeField] private float energyConsumption = 200f;
    private EventBus eventBus;
    private MoveSignal moveSignal;
    private Vector2 input;
    private float koefizientGiveEnergy = 1f;
    private float time = 0f;
    public void Init()
    {
        eventBus = ServiceLocator.Current.Get<EventBus>();
        moveSignal = new MoveSignal();
        SimpleEventBus.GetEnergy += GetCurrentEnergy;
        SimpleEventBus.GiveInput += GiveInput;
        SimpleEventBus.GiveTankId += ChangeKoefEnergy;

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
        SimpleEventBus.GiveTankId -= ChangeKoefEnergy;
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
        if (koefizientGiveEnergy < 1f)
        {
            time -= Time.deltaTime;

            if (time <= 0f)
            {
                time = 0f;
                koefizientGiveEnergy = 1f;
            }
        }
        float generatedEnergy = moveSignal.Speed;
        currentEnergy += koefizientGiveEnergy * generatedEnergy * Time.deltaTime;

        float inputPower = Mathf.Clamp01(input.magnitude);
        if (inputPower > 0.01f && currentEnergy > 0f)
        {
            float energyCost = energyConsumption * inputPower * Time.deltaTime;
            currentEnergy -= energyCost;

            if (currentEnergy < 0f)
                currentEnergy = 0f;
        }

        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);


        if (slider != null)
            slider.value = currentEnergy;
    }

    private void ChangeKoefEnergy(ITankId tankId)
    {
        koefizientGiveEnergy -= 0.2f;
        time = Const.TimeForDeceleration;
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
