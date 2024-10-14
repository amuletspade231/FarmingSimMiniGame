using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwo : MonoBehaviour
{
    public KeyCode left = KeyCode.LeftArrow;
    public KeyCode right = KeyCode.RightArrow;
    public bool phaseTwoActive = false;

    public float barTotal = 100;
    public float barGoal = 70;
    [Range(0f, 100f)]
    public float barCurrent = 0;
    public float barGainMultiplier = 8;
    [Tooltip("How much time it takes for the score to be penalized when not in the bar sweet spot")]
    public float timeDelay = 0.5f;
    private float lastTime;

    [Header("Score")]
    public float scoreTotal = 0;
    public float scoreMaxGain = 7;
    public float scoreSmallGain = 3;
    public float scorePenalty = 5;

    void Start()
    {
        // For ease of understanding, the multiplier is a small number
        // Multiply it here for better effect
        barGainMultiplier *= 100;
    }

    void Update()
    {
        // When phase two is active, start running
        if (phaseTwoActive)
        {
            PhaseTwoAction();
        }
    }


    // Actions for handeling phase two
    public void PhaseTwoAction()
    {   
        // Check if the input is the arrow keys
        if (Input.GetKeyDown(left) || Input.GetKeyDown(right))
        {     
            // Add to the bar total based on how much time has passed, multiplied by some multiplier
            barCurrent += Time.deltaTime * barGainMultiplier; 
        }

        // If the current time is ready to calculate the score
        if (Time.time >= lastTime + timeDelay)
        {
            // Calculate the score and reset the time until next calculation
            CalculateScore();
            lastTime = Time.time;
        }
    }

    private void CalculateScore()
    {
        // Checks where the user is compared to the values on the bar
        if (barCurrent >= 65 && barCurrent <= 75)
        {
            // User is in sweet spot - add max score
            scoreTotal += scoreMaxGain;
        }
        else if (barCurrent >= 45 && barCurrent <= 95)
        {
            // User is near the sweet spot - add some points
            scoreTotal += scoreSmallGain;
        }
        else
        {
            // The user is losing points
            // Check if they are at or near 0
            if (scoreTotal <= 5)
            {
                scoreTotal = 0;
            }
            else
            {
                scoreTotal -= scorePenalty;
            }
        }
    }
}
