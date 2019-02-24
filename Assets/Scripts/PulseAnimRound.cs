using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAnimRound : MonoBehaviour
{

    Animator animPulse;
    // Start is called before the first frame update
    void Start()
    {
        animPulse = GetComponent<Animator>();
    }


    public static void AnimPulse()
    {
       // animPulse.SetTrigger("Pulse");
    }
}
