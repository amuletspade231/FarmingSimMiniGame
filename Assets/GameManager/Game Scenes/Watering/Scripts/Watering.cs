using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watering : MonoBehaviour
{
    // Create a public variable to hold the nextState information in case it needs to be specified by a designer
    public GameManager.GameState nextState = GameManager.GameState.TRANSITION_TO_WEEDING;

    // Designer-specified timer length for current state
    public float stateTimeDuration;

    public KeyCode left = KeyCode.LeftArrow;
    public KeyCode right = KeyCode.RightArrow;
    public bool phaseTwoActive = false;

    public float barTotal = 10;
    [Tooltip("How much each arrow press raises the bar")]
    public float barGainMultiplier = 11;

    [Range(0f, 10f)]
    public float barCurrent = 0;
    [Range(0f, 10f)]
    public float sweetSpotLowerBound = 6;
    [Range(0f, 10f)]
    public float sweetSpotUpperBound = 8;
    private float sweetSpotMiddle;

    [Header("Score")]
    public int scoreTotal;
    public int scorePenalty = 5;
    [Tooltip("How much time it takes for the score to be penalized when not in the bar sweet spot")]
    public float timeDelay = 0.5f;
    private float lastTime;

    // Start is called before the first frame update
    void Start()
    {
        // For ease of understanding, the multiplier is a small number
        // Multiply it here for better effect
        barGainMultiplier *= 10;

        // Set this state's maxTime and set timer to active
        // Make sure there is a timer and game manager before calling it
        if (Timer.instance != null && GameManager.instance != null)
        {
            Timer.instance.isTimerActive = true;
            Timer.instance.newStateTimer(stateTimeDuration);

            scoreTotal = GameManager.instance.currentScore;
        }        

        phaseTwoActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        // When phase two is active, start running
        if (phaseTwoActive)
        {
            PhaseTwoAction();
        }
        
        // Check whether time has expired...
        if (Timer.instance != null && !Timer.instance.isTimerActive && GameManager.instance != null)
        {
            // Calculate the final score before switching states
            phaseTwoActive = false;
            CalculateScore();
            GameManager.instance.currentScore = scoreTotal;
            GameManager.instance.ChangeState(nextState);
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
            // Null check the game manager
            if (GameManager.instance != null)
            {
                GameManager.instance.currentScore = scoreTotal;
            }
            lastTime = Time.time;
        }
    }

    private void CalculateScore()
    {
        // At end phase, calculate the final score via multiplier
        if (!phaseTwoActive)
        {
            // If player is in the sweet spot, double their score
            if (barCurrent >= sweetSpotLowerBound && barCurrent <= sweetSpotUpperBound)
            {
                scoreTotal *= 2;
            }
            else // Otherwise calculate the percent increase based on how close they are to the middle of the sweet spot
            {
                // Find the middle of the sweet spot - find how far away the player is - convert it to a decimal and multiply score
                sweetSpotMiddle = (sweetSpotUpperBound + sweetSpotLowerBound) / 2;
                float temp = ((barCurrent % sweetSpotMiddle) / 10) + 1;
                temp = scoreTotal * temp;
                scoreTotal = (int)temp;
            }
            

        }
        // Checks where the user is compared to the values on the bar
        else if (barCurrent < sweetSpotLowerBound || barCurrent > sweetSpotUpperBound)
        {
            // User is not in sweet spot - decrement their score
            // Check if they are at or near 0
            if (scoreTotal <= scorePenalty)
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
