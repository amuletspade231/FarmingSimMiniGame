using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIUIManager : MonoBehaviour
{
    public Text timerText;
    public Text AIscoreText;
    public SeedingAI AI;
    private Timer timer;
    private Seeding seed;
    public int AIScoreCurr;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "Time: 0";
        AIscoreText.text = "Score: 0";
        timer = FindObjectOfType<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the timer is active
        if (timer.isTimerActive)
        {
            AIScoreCurr = AI.AIScore;
            timerText.text = "Time: " + ((int)timer.currentTime).ToString();
            AIscoreText.text = "Score: " + AIScoreCurr.ToString();
        }
    }
}
