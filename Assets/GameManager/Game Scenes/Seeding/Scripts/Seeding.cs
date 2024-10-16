using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeding : MonoBehaviour
{
    // Create a public variable to hold the nextState information in case it needs to be specified by a designer
    public GameManager.GameState nextState = GameManager.GameState.TRANSITION_TO_WATERING;

    public KeyCode input = KeyCode.Space;
    public bool phaseOneActive;

    private float barTotal = 10;
    public float barCurrent = 0;

    [Header("Score")]
    public int scoreTotal = 0;

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
                    barCurrent += Time.deltaTime * 5;

                    // If the bar reaches the max, start decreasing back to 0
                    if (barCurrent > barTotal)
                    {
                        increse = false;
                    }
                }

                // Decrease the bar from 10 to 0 based on how much time has passed
                else if (!increse)
                {
                    barCurrent -= Time.deltaTime * 5;

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
                GameManager.instance.currentScore = scoreTotal;

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
        if (barCurrent >= 4.5f && barCurrent <= 5.5f)
        {
            scoreTotal += 4;
        }
        else if (barCurrent >= 3 && barCurrent <= 7)
        {
            scoreTotal += 2;
        }
        else if (barCurrent >= 1 && barCurrent <= 9)
        {
            scoreTotal += 1;
        }

        // Adding checking later for if the phase is over or not
        phaseOneActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Check whether time has expired...
        if (!Timer.instance.isTimerActive)
        {
            phaseOneActive = false;
            GameManager.instance.ChangeState(nextState);
        }
    }
}
