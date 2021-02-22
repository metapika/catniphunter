using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTrigger : MonoBehaviour
{
    public int abilityID1;
    public int abilityID2;
    public bool alreadyTriggered = false;
    private void Start() {
        abilityID1 = Random.Range(0, 4);
        abilityID2 = Random.Range(0, 4);
    }

    // private void OnTriggerEnter(Collider other) {
    //     if(other.CompareTag("Player")) {
    //         if(alreadyTriggered == false) {
    //             if(other.GetComponent<PlayerPhysics>().IsGrounded()){
    //                 alreadyTriggered = true;
    //                 TriggerDialogue();
    //             }
    //         }
    //     }
    // }

    public void TriggerDecision() 
    {
        FindObjectOfType<DecisionController>().StartDecision(abilityID1, abilityID2);
        //Destroy(gameObject);
    }
}
