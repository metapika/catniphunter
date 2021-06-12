using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueManager dialogueManager;
    private void Update() {
        if(dialogueManager == null) return;

        if(Input.GetKeyDown(KeyCode.F)) {
            dialogueManager.DisplayNextSentence();
        }
    }
}
