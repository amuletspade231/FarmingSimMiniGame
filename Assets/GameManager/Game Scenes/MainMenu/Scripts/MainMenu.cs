using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Create a public variable to hold the nextState information in case it needs to be specified by a designer
    public GameManager.GameState nextState = GameManager.GameState.GAME_START;

    // Start is called before the first frame update
    void Start()
    {
        // Set currentState to MAIN_MENU in the event of an exception during state handling
        GameManager.instance.currentState = GameManager.GameState.MAIN_MENU;

        // Set this state's maxTime and set timer to active - this is temporary as it's the main menu
        Timer.instance.isTimerActive = true;
        Timer.instance.newStateTimer(5f);

        // TODO: Implement logic for the display of Main Menu UI
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