using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreText : MonoBehaviour
{
    private scoreManager scoreM;
    public Text P1scoreT;
    public Text P2scoreT;
    public Text P3scoreT;
    public Text P4scoreT;
    // Start is called before the first frame update
    void Start()
    {
        scoreM = FindObjectOfType<scoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        P1scoreT.text = scoreM.P1Score.ToString();
        P2scoreT.text = scoreM.P2Score.ToString();
        P3scoreT.text = scoreM.P3Score.ToString();
        P4scoreT.text = scoreM.P4Score.ToString();
    }
}
