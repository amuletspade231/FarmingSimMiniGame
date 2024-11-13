using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text timerText;
    public Text scoreText;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "Time: 0";
        scoreText.text = "Score: 0";
        timer = FindObjectOfType<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the timer is active
        if (timer.isTimerActive)
        {
            timerText.text = "Time: " + ((int)timer.currentTime).ToString();
        }
    }

    // Update the score on the UI
    public void UpdateScoreUI(int score) 
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
