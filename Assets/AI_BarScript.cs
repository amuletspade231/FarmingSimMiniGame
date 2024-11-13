using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AI_UI_Script : MonoBehaviour
{
    public SeedingAI SAI;
    public Slider slider;
    private Seeding Seed;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = SAI.AIBar;
        slider.maxValue = Seed.barTotal;
    }
    // Update is called once per frame
    void Update()
    {
        slider.value = SAI.AIBar;
    }
}
