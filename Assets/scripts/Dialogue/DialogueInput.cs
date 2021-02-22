using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInput : MonoBehaviour
{
    public bool alreadyTriggered = false;
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            if(alreadyTriggered == false) {
                alreadyTriggered = true;
                GetComponent<DialogueTrigger>().TriggerDialogue();
            }
        }
    }
}
