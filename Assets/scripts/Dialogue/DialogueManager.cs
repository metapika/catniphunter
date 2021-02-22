using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private Queue<string> sentences;
    
    void Start() {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue) {
        dialogueText.transform.parent.parent.gameObject.SetActive(true);
        dialogueText.transform.parent.parent.GetComponent<Animator>().SetTrigger("start");
        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence () {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        dialogueText.transform.parent.parent.GetComponent<Animator>().SetTrigger("end");

        DecisionTrigger decision = null; 

        if(GetComponent<DecisionTrigger>() != null) {
            decision = GetComponent<DecisionTrigger>();
            decision.TriggerDecision();
        }
    }
}
