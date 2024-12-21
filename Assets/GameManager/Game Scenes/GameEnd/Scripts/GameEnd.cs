using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    // Create a public variable to hold the nextState information in case it needs to be specified by a designer
    public GameManager.GameState nextState = GameManager.GameState.MAIN_MENU;

    // Designer-specified timer length for current state
    public float stateTimeDuration;

    // Start is called before the first frame update
    void Start()
    {
        // Set this state's maxTime and set timer to active
        Timer.instance.isTimerActive = true;
        Timer.instance.newStateTimer(stateTimeDuration);
        AudioManager.Instance.Stop("bkg_music");
    }

    public void ResetGame()
        {
            GameManager.instance.ChangeState(nextState);
            GameManager.instance.RandomHighScore();
        }
}
