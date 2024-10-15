using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWaterScript : MonoBehaviour
{
    public int targetWaterAmount;
    public int currentwaterAmount;
    public WaterUIScript waterUI;
    public int randomInt;
    // Start is called before the first frame update

    void Start()
    {  
        int randomInt = Random.Range(50,100);
        currentwaterAmount = 0;
        targetWaterAmount = randomInt;
        waterUI.SetTargetWaterLevel(randomInt);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            addWater(5);
        }

    }
    void addWater(int watering)
    {
        currentwaterAmount += watering;
        waterUI.SetwaterAmount(currentwaterAmount);
    }
}
