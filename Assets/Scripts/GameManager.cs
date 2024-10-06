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

    // Variables to store player name and high score
    private string playerName = "Player1";
    private int currentScore = 0;
    private int highScore = 0;

    // DEBUGGING: Array of random names for testing purposes
    private string[] randomNames = { "Alex", "Jordan", "Taylor", "Casey", "Morgan", "Riley", "Cameron", "Quinn", "Skylar", "Avery" };

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

        // Load high score
        LoadHighScore();
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

                    // This is for DEBUGGING PURPOSES, it will not be implemented in the final version of the project
                    int randomScore = UnityEngine.Random.Range(0, 1000); // Random score
                    Debug.Log($"Generated random score: {randomScore}");

                    // DEBUGGING: Pick a random name from the list of names
                    string randomName = randomNames[UnityEngine.Random.Range(0, randomNames.Length)];
                    Debug.Log($"Generated random name: {randomName}");

                    // Call the EndGame method to save the score
                    EndGame(randomName, randomScore);
                    break;
                default:
                    Debug.LogError($"Unexpected previous state: {previousState}");
                    break;
            }
        }

        // Log the transition
        Debug.Log($"Transitioned to state: {currentState}");
    }

    // This method is for DEBUGGING PURPOSES.  It will not be implemented in the final version of the project.
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Current State: {currentState}");
        GUI.Label(new Rect(10, 30, 200, 20), $"Time in State: {timeInState:F2}s / {(currentState == StateType.TRANSITION ? transitionDuration : stateDuration)}s");

        // Display high score and player name
        GUI.Label(new Rect(10, 50, 200, 20), $"Player Name: {playerName}");
        GUI.Label(new Rect(10, 70, 200, 20), $"High Score: {highScore}");
    }

    // Save the player's name and high score using PlayerPrefs
    public void SaveHighScore(string name, int score)
    {
        playerName = name;
        currentScore = score;

        highScore = currentScore;
        PlayerPrefs.SetString("HighScoreName", playerName);
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
        Debug.Log($"New high score saved: {playerName} - {highScore}");
    }

    // Load the player's name and high score using PlayerPrefs
    public void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        playerName = PlayerPrefs.GetString("HighScoreName", "Unknown");
        Debug.Log($"Loaded high score: {playerName} - {highScore}");
    }

    // Example method to simulate end of the game and save the score
    public void EndGame(string playerName, int finalScore)
    {
        // Call SaveHighScore at the end of the game to update high score if necessary
        if (currentScore > highScore)
        {
            SaveHighScore(playerName, finalScore);
        }
    }
}