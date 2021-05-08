using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    public float resetTime;

    [Space]

    public bool canParry;
    Coroutine timerCoroutine;
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("The key " + KeyCode.U + " is DOWN!");

            if(timerCoroutine == null) 
            {
                timerCoroutine =  StartCoroutine(ButtonClicked(resetTime));
            } else {
                StopCoroutine(timerCoroutine);
                timerCoroutine =  StartCoroutine(ButtonClicked(resetTime));
            }
        }
    }

    private IEnumerator ButtonClicked(float time)
    {
        Debug.Log("The Coroutine has started");

        canParry = true;

        yield return new WaitForSeconds(time);

        canParry = false;
        
        Debug.Log("Successfully turned canParry to false in " + time);
    }
}
