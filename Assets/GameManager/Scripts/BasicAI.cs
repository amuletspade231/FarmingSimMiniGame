using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAI : MonoBehaviour
{
    public enum difficulty { BEGINNER, NORMAL, HARD };
    public int AIScore;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public abstract void MakeDecisions();
}
