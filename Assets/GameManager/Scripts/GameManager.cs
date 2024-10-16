using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Declare the instance used to contain the game manager
    public static GameManager instance { get; private set; }

    // Reference to the Timer script
    private Timer timer;

    // Enum for game states
    public enum GameState
    {
        MAIN_MENU,
        GAME_START,
        SEEDING_STATE,
        TRANSITION_TO_WATERING,
        WATERING_STATE,
        TRANSITION_TO_WEEDING,
        WEEDING_STATE,
        GAME_END
    }

    // Variables to track current game state and new game state
    public GameState currentState = GameState.MAIN_MENU;
    private GameState newState;

    // DEBUG: Array of names to randomly select for save/load testing
    public string[] testNames = {
        "Alice",
        "Bob",
        "Charlie",
        "Diana",
        "Ethan",
        "Fiona",
        "George",
        "Hannah",
        "Ian",
        "Julia"
    };

    private string selectedName;

    // Variables to track name and score for save/load of high score information
    public string playerName = "Unknown";
    public int currentScore = 0;
    public int highScore = 0;

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
        Debug.Log("GameManager Start() called.");

        // Get the Timer component from the scene
        timer = FindObjectOfType<Timer>();

        // Start with the MAIN_MENU state
        ChangeState(GameState.MAIN_MENU);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ----------------------------------------------------------------------
    //   State Machine methods
    // ----------------------------------------------------------------------

    public void ChangeState(GameState newState)
    {
        Debug.Log($"Changing state to {newState}");

        // Change the game state
        currentState = newState;
        HandleStateChange(currentState);
    }

    void HandleStateChange(GameState state)
    {
        Debug.Log($"Handling state change: {state}");

        try
        {
            switch (state)
            {
                case GameState.MAIN_MENU:
                    LoadScene("MainMenu");
                    break;
                case GameState.GAME_START:
                    LoadScene("GameStart");
                    break;
                case GameState.SEEDING_STATE:
                    LoadScene("Seeding");
                    break;
                case GameState.TRANSITION_TO_WATERING:
                    LoadScene("WateringTransition");
                    break;
                case GameState.WATERING_STATE:
                    LoadScene("Watering");
                    break;
                case GameState.TRANSITION_TO_WEEDING:
                    LoadScene("WeedingTransition");
                    break;
                case GameState.WEEDING_STATE:
                    LoadScene("Weeding");
                    break;
                case GameState.GAME_END:
                    LoadScene("GameEnd");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, $"Unhandled game state: {state}");
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.LogError($"State machine error: {ex.Message}");
            // Return to the main menu
            LoadScene("MainMenu");
        }
    }

    void LoadScene(string sceneName)
    {
        // Handle the scene loading based on state
        SceneManager.LoadScene(sceneName);
    }

    // ----------------------------------------------------------------------
    //   Save data methods
    // ----------------------------------------------------------------------

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


    // ----------------------------------------------------------------------
    //   This is for DEBUGGING PURPOSES ONLY
    //   - OnGUI information via GUI Labels in order to maintain visual
    //     reference to needed information that would otherwise be invisible
    //   - RandomHighScore generates a random name and score combination to
    //     test the logic that compares scores and saves persistent data
    // 
    //   This should not be implemented in the final release of the project
    // ----------------------------------------------------------------------
    void OnGUI()
    {
        // Display current state
        GUI.Label(new Rect(10, 10, 400, 20), $"Current State: {currentState}");
        GUI.Label(new Rect(10, 50, 400, 20), $"Timer: {timer} - isActive [{Timer.instance.isTimerActive}]");
        GUI.Label(new Rect(10, 70, 200, 20), $"Time in State: {Timer.instance.GetCurrentTime():F2}s / {Timer.instance.GetMaxTime()}s");

        // Display high score and player name
        GUI.Label(new Rect(10, 110, 200, 20), $"Player Name: {playerName}");
        GUI.Label(new Rect(10, 130, 200, 20), $"High Score: {highScore}");
        GUI.Label(new Rect(10, 150, 200, 20), $"Score: {currentScore}");
    }

    public void RandomHighScore()
    {
        int simulatedScore;

        // Randomly select a name from the array
        selectedName = testNames[UnityEngine.Random.Range(0, testNames.Length)];
        simulatedScore = UnityEngine.Random.Range(0, 1000);
        currentScore = simulatedScore;

        EndGame(selectedName, simulatedScore);
    }
}