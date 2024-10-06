using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public static Timer instance { get; private set; }

    private float maxTime;
    private float currentTime;
    public bool isTimerActive;

    //public TextMeshPro timerUI;


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
        // Set the current time to the max on start
        currentTime = maxTime;

        //isTimerActive = false;    - set to false once a function can call to activate the timer
    }

    // Update is called once per frame
    void Update()
    {
        // While the timer is active and it is greater than 0
        if (isTimerActive && currentTime > 0)
        {
            // Decrement the timer
            currentTime -= Time.deltaTime;
            // Update the UI
            updateTimerUI();
        }
        else // Otherwise the timer should not be active
            isTimerActive = false;
    }


    // Function to start the countdown timer
    public void startTimer()
    {
        updateTimerUI();
        isTimerActive = true;
    }

    // Get the current time
    public float GetCurrentTime()
    {
        return currentTime;
    }

    public float GetMaxTime()
    {
        return maxTime;
    }

    // Function to update the timer UI element
    private void updateTimerUI()
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        //timerUI.text = time.ToString("ss':'ff");

        Debug.Log(time.Seconds.ToString());
        //Debug.Log(time.ToString("ss':'ff")); formatting with miliseconds
    }

    public void newStateTimer(float newTime)
    {
        changeMaxTime(newTime);
        setTimerToMax();
    }

    // Function to set the max time to the default set in inspector
    private void setTimerToMax() 
    {
        currentTime = maxTime;
    }
    
    // Function to set the max time by taking in a new max value
    private void setTimerToMax(float in_max) 
    {
        currentTime = in_max;
    }

    // Function to change the max amount of time on the timer
    private void changeMaxTime(float in_max)
    {
        maxTime = in_max;
    }

    // Function to check if the timer is up
    private bool isTimerDone() 
    {
        if (currentTime <= 0)
            return true;
        else 
            return false;
    }
}