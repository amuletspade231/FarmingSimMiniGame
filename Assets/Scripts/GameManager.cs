using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Declare the instance used to contain the game manager
    public static GameManager instance { get; private set; }

    // Enum for game states
    public enum StateType
    {
        DEFAULT,    // Fallback state - should never happen
        MAINMENU,   // Main menu
        GAMESTART,  // The start of the minigame, executes countdown to first phase
        SEEDING,    // The seeding state of the minigame
        WATERING,   // The watering state of the minigame
        WEEDING,    // The weeding state of the minigame
        GAMEEND,    // The end state of the minigame
        GAMEOVER,   // Display the player's score, high scores, and give option to restart or quit
    }

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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}