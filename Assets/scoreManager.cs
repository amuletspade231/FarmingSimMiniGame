using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreManager : MonoBehaviour
{
    public Seeding seed;
    public SeedingAI Player2;
    public SeedingAI Player3;
    public SeedingAI Player4;
    public int P1Score;
    public int P2Score;
    public int P3Score;
    public int P4Score;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
      P1Score = seed.scoreTotal;  
      P2Score = Player2.AIScore;
      P3Score = Player3.AIScore;
      P4Score = Player4.AIScore;
    }


}
