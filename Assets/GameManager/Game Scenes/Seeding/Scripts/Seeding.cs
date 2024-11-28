using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeding : MonoBehaviour
{
    // Create a public variable to hold the nextState information in case it needs to be specified by a designer
    public GameManager.GameState nextState = GameManager.GameState.TRANSITION_TO_WATERING;

    private UIManager ui;

    private HandAnimation HAnim;
    public KeyCode input = KeyCode.Space;
    public bool phaseOneActive;

    public float barTotal = 10;
    // Display bar range for designer purposes
    [Range(0f, 10f)]
    public float barCurrent = 0;
    [Tooltip("Changes how fast or how slow the bar moves when holding space")]
    public float barSpeedMultiplier = 5;

    [Header("Adjust Bar Values")]
    [Range(0f, 10f)]
    public float outsideLowerBound = 2;
    [Range(0f, 10f)]
    public float outsideUpperBound = 9;
    [Range(0f, 10f)]
    public float insideLowerBound = 4;
    [Range(0f, 10f)]
    public float insideUpperBound = 6;

    [Header("Score")]
    public int scoreTotal = 0;
    public int outsideScoreToAdd = 3;
    public int insideScoreToAdd = 6;
    public int outOfBoundsScoreToAdd = 1;

    // Designer-specified timer length for current state
    public float stateTimeDuration;

    // Start is called before the first frame update
    void Start()
    {
        // Set this state's maxTime and set timer to active
        Timer.instance.isTimerActive = true;
        Timer.instance.newStateTimer(stateTimeDuration);
        phaseOneActive = true;
        StartCoroutine(PhaseOneInput());

        ui = FindObjectOfType<UIManager>();
    }


    // Create coroutine to check for input while phase one is active
    private IEnumerator PhaseOneInput()
    {
        bool increse = true;
        // While the phase is active
        while (phaseOneActive)
        {
            // Check if the input is the space bar
            if (Input.GetKey(input))
            {
                // If the bar should be increasing
                if (increse)
                {
                    // Add to the bar total based on how much time has passed, multiplied by 5
                    barCurrent += Time.deltaTime * barSpeedMultiplier;

                    // If the bar reaches the max, start decreasing back to 0
                    if (barCurrent > barTotal)
                    {
                        increse = false;
                    }
                }

                // Decrease the bar from 10 to 0 based on how much time has passed
                else if (!increse)
                {
                    barCurrent -= Time.deltaTime * barSpeedMultiplier;

                    // Once the bar hits 0, start incrementing back to the max
                    if (barCurrent < 0)
                    {
                        increse = true;
                    }
                }
            }

            // Check for end condition
            if (Input.GetKeyUp(input))
            {
                // Add to the users score
                phaseOneActive = false;
                CalculateScore();
                // Null check the game manager
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentScore = scoreTotal;
                }

                // Reset the current bar position to 0
                barCurrent = 0;
            }

            // Wait for next frame
            yield return null;
        }
    }

    private void CalculateScore()
    {
        // Based on how close the user was to certain increments in the bar, add to their score
        if (barCurrent >= insideLowerBound && barCurrent <= insideUpperBound)
        {
            scoreTotal += insideScoreToAdd;
        }
        else if (barCurrent >= outsideLowerBound && barCurrent <= outsideUpperBound)
        {
            scoreTotal += outsideScoreToAdd;
        }
        else if (barCurrent < outsideLowerBound || barCurrent > outsideUpperBound)
        {
            scoreTotal += outOfBoundsScoreToAdd;
        }

        // Update the score UI
        ui.UpdateScoreUI(scoreTotal);

        // Adding checking later for if the phase is over or not
        phaseOneActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Check whether time has expired...
        if (!Timer.instance.isTimerActive && GameManager.instance != null)
        {
            GameManager.instance.currentScore = scoreTotal;
            phaseOneActive = false;
            GameManager.instance.ChangeState(nextState);
        }
    }
}
