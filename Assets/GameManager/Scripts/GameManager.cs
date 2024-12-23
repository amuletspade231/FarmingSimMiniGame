// Game Manager
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEditor;
using Unity.Netcode;

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

    // Multiplayer Variables
    public NetworkVariable<GameState> currentNetworkState = new NetworkVariable<GameState>(GameState.MAIN_MENU);
    [Serialize] public bool isMultiplayer = false;
    public int maxPlayers = 4;
    public Dictionary<ulong, PlayerData> Players = new Dictionary<ulong, PlayerData>();

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
        // Check if the game is multiplayer
        if (isMultiplayer)
        {
            // Only the host should trigger the scene change
            if (NetworkManager.Singleton.LocalClientId == 0) // Check if it's the host
            {
                Debug.Log("This is the host, triggering scene change.");

                // Sync game state across the network (host sets the network state)
                if (NetworkManager.Singleton.IsHost)
                {
                    currentNetworkState.Value = newState;  // Sync the new state to all clients
                    Debug.Log($"Host called server for scene change, setting network state to: {newState}");
                }
            }
            else
            {
                Debug.Log("This is not the host, waiting for host to change the scene.");
                // Clients will wait for the host to change the scene.
                return;
            }
        }

        // Regardless of whether it's multiplayer or not, handle the state change
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
        if (isMultiplayer == true)
        {
            Debug.Log("Multiplayer is true, loading network scene change..");
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log($"Host told server to change scenes. Scene is changing to: {sceneName} scene");
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }                                                   
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
    public bool AddPlayer(ulong clientId, string playerNumber)
    {
        if (Players.Count >= maxPlayers || Players.ContainsKey(clientId))
        {
            return false;
        }

        Players[clientId] = new PlayerData { ClientId = clientId, PlayerName = playerNumber};
        Debug.Log($"Player: " + playerNumber + " - Added. Player count = " + Players.Count);
        return true;
    }

    public void RemovePlayer(ulong clientId, string playerNumber)
    {
        if (Players.ContainsKey(clientId))
        {
            Players.Remove(clientId);
            Debug.Log($"Player: " + playerNumber + " - Removed. Player count = " + Players.Count);
        }
    }

    // Gathers all player data for each player
    // And returns a list of the player's data values that can be iterated through or checked with ".ContainsKey" for values
    public List<PlayerData> GetAllPlayerData()
    {
        return new List<PlayerData>(Players.Values);
    }

}

