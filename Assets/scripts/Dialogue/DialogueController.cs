using UnityEngine;
using UnityCore.Menu;
public class DialogueController : MonoBehaviour
{
    public DialogueManager dialogueManager;
    private void Update() {
        if(dialogueManager == null) return;

        if(Input.GetKeyDown(KeyCode.E)) {
            if(PageController.instance) 
            {
                if(PageController.instance.GetComponent<PauseMenu>().gameEnded) return;
            }
            dialogueManager.DisplayNextSentence();
        }
    }
}
