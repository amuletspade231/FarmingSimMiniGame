using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTimer : MonoBehaviour
{
    
    public float time = 10;


    IEnumerator Start ()
    {

        yield return new WaitForSeconds(time);
        Destroy(gameObject);

    }
}
