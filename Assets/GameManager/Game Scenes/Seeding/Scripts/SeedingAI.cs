using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class SeedingAI : BasicAI
{
    [Range(0f, 10f)]
    public float AIBar;
    private Seeding state;
    private bool isHoldingSpace = false;
    private float nextActionTime = 0f;
    private bool increase = true;
    private AIUIManager AIS;
    private Animator anim;
    public GameObject HandCursor;
    public AudioSource AudSource;

    // Start is called before the first frame update
    protected override void Start()
    {
        state = FindObjectOfType<Seeding>();
        AIBar = 0;
        base.Start();
        anim = HandCursor.GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // If there is the state, see if it is active
        if (state != null)
        {
            // If it is active, tell the AI to make game decisions
            if (state.phaseOneActive)
            {
                MakeDecisions();
            }
        }

        base.Update();
    }

    public override void MakeDecisions()
    {
        // Check if it is time to swap actions and the ai can hold the space
        if (Time.time <= nextActionTime && isHoldingSpace)
        {
            // If so, simulate holding the space bar
            SimulateSpacebar();
        }
        else // Otherwise they should not be pressing the space bar
        {
            // If they just got done holding the space bar
            if (isHoldingSpace)
            {
                // Set holding space bar to false
                isHoldingSpace = false;
                // Set the downtime in between holding space
                nextActionTime = Time.time + Random.Range(0.5f, 1f);
                // Calculate score
                CalculateAIScore();
                // Reset the current bar position and if the bar is increasing or decreasing
                AIBar = 0;
                increase = true;
            }
            // Otherwise AI is waiting to do something
            // Check if it is time to press space
            else if (Time.time > nextActionTime && !isHoldingSpace)
            {
                // Set how long they hold space and that they are doing so
                nextActionTime = Time.time + Random.Range(2f, 3f);
                isHoldingSpace = true;
            }            
        }
    }

   
    // Create coroutine to check for input while phase one is active
    private void SimulateSpacebar()
    {
        
        // If the bar should be increasing
        if (increase)
        {
            // Add to the bar total based on how much time has passed, multiplied by a multiplier
            AIBar += Time.deltaTime * state.barSpeedMultiplier;

            // If the bar reaches the max, start decreasing back to 0
            if (AIBar > state.barTotal)
            {
                increase = false;
            }
        }

        // Decrease the bar from 10 to 0 based on how much time has passed
        else if (!increase)
        {
            AIBar -= Time.deltaTime * state.barSpeedMultiplier;

            // Once the bar hits 0, start incrementing back to the max
            if (AIBar < 0)
            {
                increase = true;
            }
        }     
    }

    public void CalculateAIScore()
    {
        // Based on how close the AI was to certain increments in the bar, add to their score
        if (AIBar >= state.insideLowerBound && AIBar <= state.insideUpperBound)
        {
            AIScore += state.insideScoreToAdd;
            anim.Play("HandPlantingMiddle");
            AudSource.Play();

        }
        else if (AIBar >= state.outsideLowerBound && AIBar <= state.outsideUpperBound)
        {
            AIScore += state.outsideScoreToAdd;
            anim.Play("HandPlantingMiddleMistake");
            AudSource.Play();
        }
        else if (AIBar < state.outsideLowerBound || AIBar > state.outsideUpperBound)
        {
            AIScore += state.outOfBoundsScoreToAdd;
            anim.Play("HandPlantingMiddleMistake");
            AudSource.Play();
        }
    }
}
