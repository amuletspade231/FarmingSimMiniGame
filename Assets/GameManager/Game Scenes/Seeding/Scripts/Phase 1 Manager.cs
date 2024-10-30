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
    public int currTimeInt;
    // Start is called before the first frame update
    void Start()
    {
        currTime = timer.currentTime;
        phaseOneScore = 0;
        currTimeInt = (int)currTime;
    }

    // Update is called once per frame
    void Update()
    {
        timeText.text = "Time: " + currTimeInt.ToString();
        currTime = timer.currentTime;
        currTimeInt= (int)currTime;
        phaseOneScore = p1.scoreTotal;
        scoreText.text = "Score: " + phaseOneScore.ToString();
    }
}
