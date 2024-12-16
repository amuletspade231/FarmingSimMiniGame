using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{


    [SerializeField] public Button multiplayer;

    // Referencing the multiplayer script
    [CanBeNull]
    public MultiplayerMenu multiplayerMenu;

    // Create a public variable to hold the nextState information in case it needs to be specified by a designer
    public GameManager.GameState nextState = GameManager.GameState.GAME_START;

    // Start is called before the first frame update
    void Start()
    {
        // Set currentState to MAIN_MENU in the event of an exception during state handling
        GameManager.instance.currentState = GameManager.GameState.MAIN_MENU;

        // Set this state's maxTime and set timer to active - this is temporary as it's the main menu
        Timer.instance.isTimerActive = true;
        Timer.instance.newStateTimer(99999F);

        // TODO: Implement logic for the display of Main Menu UI


        //Event listener for when Menu UI is done
        // Null check button asset
        if (multiplayer != null)
        {
            multiplayer.onClick.AddListener(OpenMultiplayerMenu);
        }

    }

    // Update is called once per frame


    public void OpenMultiplayerMenu() // Loading the scene can be assigned to menu UI later
        {
            SceneManager.LoadScene("MultiplayerMenu");   
        }
    public void PlayGame()
    {
        GameManager.instance.ChangeState(nextState);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}