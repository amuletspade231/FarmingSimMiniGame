using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedingTransition : MonoBehaviour
{
    // Create a public variable to hold the nextState information in case it needs to be specified by a designer
    public GameManager.GameState nextState = GameManager.GameState.WEEDING_STATE;

    // Designer-specified timer length for current state
    public float stateTimeDuration;

    // Start is called before the first frame update
    void Start()
    {
        // Set this state's maxTime and set timer to active
        Timer.instance.isTimerActive = true;
        Timer.instance.newStateTimer(stateTimeDuration);
    }

    // Update is called once per frame
    void Update()
    {
        // Check whether time has expired...
        if (!Timer.instance.isTimerActive)
        {
            GameManager.instance.ChangeState(nextState);
        }
    }
}
