using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedingUIscript : MonoBehaviour
{
    public Slider slider;
    public float currBarLevel;
    public float barMax;
    public Seeding seed;
    // Start is called before the first frame update
    void Start()
    {
        barMax = seed.barTotal;
        currBarLevel = seed.barCurrent;
        slider.maxValue = barMax;
    }

    // Update is called once per frame
    void Update()
    {
        currBarLevel = seed.barCurrent;
        slider.value = currBarLevel;
    }
}
