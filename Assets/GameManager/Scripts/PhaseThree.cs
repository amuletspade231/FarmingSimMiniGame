using System.Collections;
using UnityEngine;

public class Phase3_Deweed : MonoBehaviour
{
    public int weedCount = 10; // init weed count
    public int timer = 10;     // timer
    public int score = 1000;   // Player init score
    public int scorePenalty = 10; // Penalty per weed count
    public int scorePerWeed = 20; // Bonus for removing a weed
    public bool isPhaseActive = false; // Controls whether the phase is active

    void Start()
    {
        StartPhase();
    }

    void Update()
    {
        if (isPhaseActive)
        {
            // Listen for button press to reduce weed count
            if (Input.GetKeyDown(KeyCode.Space)) // Replace with gamepad button input if necessary
            {
                RemoveWeed();
            }

            // End phase if weed count is 0 or timer reaches 0
            if (weedCount <= 0 || timer <= 0)
            {
                EndPhase();
            }
        }
    }

    void StartPhase()
    {
        isPhaseActive = true;
        StartCoroutine(PhaseTimer());
    }

    IEnumerator PhaseTimer()
    {
        while (timer > 0 && weedCount > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;

            // Deduct score based on remaining weeds every second
            score -= scorePenalty * weedCount;
        }
    }

    void RemoveWeed()
    {
        if (weedCount > 0)
        {
            weedCount--;
            score += scorePerWeed; // Give player a bonus for each weed removed
            Debug.Log("Weed removed! Remaining: " + weedCount);
        }
    }

    void EndPhase()
    {
        isPhaseActive = false;

        // Optionally give bonus points for time left when weeds are cleared before timer ends
        if (weedCount <= 0 && timer > 0)
        {
            score += timer * 10; // Example code each second left gives 10 bonus points
        }

        Debug.Log("Phase 3: Deweed complete!");
        Debug.Log("Final Score: " + score);
        // Trigger any end phase logic here ? 
    }
}
