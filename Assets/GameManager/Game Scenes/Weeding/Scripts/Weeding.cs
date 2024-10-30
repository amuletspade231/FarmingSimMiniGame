using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weeding : MonoBehaviour
{
    // Create a public variable to hold the nextState information in case it needs to be specified by a designer
    public GameManager.GameState nextState = GameManager.GameState.GAME_END;

    // Designer-specified timer length for current state
    public float stateTimeDuration;

    public KeyCode input = KeyCode.Space;
    public bool phaseThreeActive = false;

    public int weedsTotal = 30;
    public int weedsPulledCurrent;
    public int weedsMissed;

    [Header("Score")]
    public int scoreTotal;
    [Tooltip("Score penalty per weed not pulled")]
    public int scorePenalty = 5;


    // Start is called before the first frame update
    void Start()
    {
        // Set this state's maxTime and set timer to active
        Timer.instance.isTimerActive = true;
        Timer.instance.newStateTimer(stateTimeDuration);

        // Get the current user score
        scoreTotal = GameManager.instance.currentScore;

        phaseThreeActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        // When phase two is active, start running
        if (phaseThreeActive)
        {
            PhaseThreeAction();
        }

        // Check whether time has expired...
        if (!Timer.instance.isTimerActive)
        {
            // Set the phase to inactive, calculate the final score and update it in the gameManager
            phaseThreeActive = false;
            CalculateScore();
            GameManager.instance.currentScore = scoreTotal;
            GameManager.instance.ChangeState(nextState);
        }
    }


    // Actions for handeling phase two
    public void PhaseThreeAction()
    {
        // Check if the input is the arrow keys
        if (Input.GetKeyDown(input))
        {
            if (weedsPulledCurrent < weedsTotal)
            {
                weedsPulledCurrent += 1;
            }
        }
    }

    private void CalculateScore()
    {
        weedsMissed = weedsTotal - weedsPulledCurrent;
        scoreTotal -= (scorePenalty * weedsMissed);
        if (scoreTotal < 0)
            scoreTotal = 0;
    }
}
