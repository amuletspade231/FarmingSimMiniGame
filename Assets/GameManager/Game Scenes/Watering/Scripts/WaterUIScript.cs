using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterUIScript : MonoBehaviour
{
    public Slider slider;
    public void SetwaterAmount(int waterAmount)
    {
        slider.value = waterAmount;

    }
    public void SetTargetWaterLevel(int targetWaterAmount)
    {
        slider.maxValue = targetWaterAmount;
    }

}
