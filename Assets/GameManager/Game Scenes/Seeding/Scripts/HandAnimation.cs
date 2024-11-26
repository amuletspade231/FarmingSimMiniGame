using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandAnimation : MonoBehaviour
{
    private Animator anim;
    public Seeding seed;
    public KeyCode input = KeyCode.Space;
    public AudioSource AudSource;

    // Start is called before the first frame update

    void Start()
    {
       anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(input)){
            animationCheck();
        }
        
    }
    
    private void animationCheck()
    {
        if (seed.barCurrent >= seed.insideLowerBound && seed.barCurrent <= seed.insideUpperBound)
        {
            anim.Play("HandPlantingMiddle");
            AudSource.Play();
        }
        else if (seed.barCurrent >= seed.outsideLowerBound && seed.barCurrent <= seed.outsideUpperBound)
        {
            anim.Play("HandPlantingMiddleMistake");
            AudSource.Play();
        }
        else if (seed.barCurrent < seed.outsideLowerBound || seed.barCurrent > seed.outsideUpperBound)
        {
            anim.Play("HandPlantingMiddleMistake");
            AudSource.Play();
        }
    }
}
