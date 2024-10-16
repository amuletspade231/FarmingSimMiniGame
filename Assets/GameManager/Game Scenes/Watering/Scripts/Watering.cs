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

    public float barTotal = 100;
    public float barGoal = 70;
    [Range(0f, 100f)]
    public float barCurrent = 0;
    public float barGainMultiplier = 8;
    [Tooltip("How much time it takes for the score to be penalized when not in the bar sweet spot")]
    public float timeDelay = 0.5f;
    private float lastTime;

    [Header("Score")]
    public int scoreTotal = 0;
    public int scoreMaxGain = 7;
    public int scoreSmallGain = 3;
    public int scorePenalty = 5;

    // Start is called before the first frame update
    void Start()
    {
        // Set this state's maxTime and set timer to active
        Timer.instance.isTimerActive = true;
        Timer.instance.newStateTimer(stateTimeDuration);

        // For ease of understanding, the multiplier is a small number
        // Multiply it here for better effect
        barGainMultiplier *= 100;

        scoreTotal = GameManager.instance.currentScore;
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
        if (!Timer.instance.isTimerActive)
        {
            phaseTwoActive = false;
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
            GameManager.instance.currentScore = scoreTotal;
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
