using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour

{
    public Timer timer;
    public PhaseOne p1;
    public Text timeText;
    public Text scoreText;
    public float currTime;
    public float phaseOneScore;
    // Start is called before the first frame update
    void Start()
    {
        currTime = timer.currentTime;
        phaseOneScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeText.text = "Time: " + currTime.ToString();
        currTime = timer.currentTime;
        phaseOneScore = p1.scoreTotal;
        scoreText.text = "Score: " + phaseOneScore.ToString();
    }
}
