using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Declare the instance used to contain the game manager
    public static GameManager instance { get; private set; }

    // Enum for game states
    public enum StateType
    {
        TRANSITION,
        SEEDING,
        WATERING,
        WEEDING
    }

    // currentState and previousState will allow us to streamline the enumerated list to prevent needing multilple different states
    // Nothing requires this to be implemented like this in the minigame's final form if it becomes prohibitive to adapt TRANSITION
    //   as a template to determine state-dependent actions
    private StateType currentState = StateType.SEEDING;
    private StateType previousState = StateType.WEEDING;

    private float timeInState = 0f;

    // Variables made accessible in the inspector for designer access
    public float stateDuration = 10f;
    public float transitionDuration = 5f;

    private void Awake()
    {
        // If there is an instance and it's not me, delete myself
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Initial state: {currentState}");
    }

    // Update is called once per frame
    void Update()
    {
        // Accumulate time
        timeInState += Time.deltaTime;

        // Check whether timeInState has reached the appropriate state duration
        if (timeInState >= (currentState == StateType.TRANSITION ? transitionDuration : stateDuration))
        {
            timeInState = 0f; // Reset timer
            TransitionToNextState();
        }
    }

    private void TransitionToNextState()
    {
        // If the current state is SEEDING, WATERING, or WEEDING, transition to the TRANSITION state
        if (currentState != StateType.TRANSITION)
        {
            previousState = currentState;
            currentState = StateType.TRANSITION;
        }
        else
        {
            // In TRANSITION, decide the next state based on the previous one
            switch(previousState)
            {
                case StateType.SEEDING:
                    currentState = StateType.WATERING;
                    break;
                case StateType.WATERING:
                    currentState = StateType.WEEDING;
                    break;
                case StateType.WEEDING:
                    // This is the last state of the minigame and will lead to the decision to either game over or restart
                    Debug.Log($"Reached final state: {currentState}.  Implement game end logic here.");
                    break;
                default:
                    Debug.LogError($"Unexpected previous state: {previousState}");
                    break;
            }
        }

        // Log the transition
        Debug.Log($"Transitioned to state: {currentState}");
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Current State: {currentState}");
        GUI.Label(new Rect(10, 30, 200, 20), $"Time in State: {timeInState:F2}s / {(currentState == StateType.TRANSITION ? transitionDuration : stateDuration)}s");
    }
}