using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    [HideInInspector] public Queue<string> sentences;
    
    void Start() {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue) {
        Time.timeScale = 0f;

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
        Time.timeScale = 1f;
        if (!dialogueText.transform.parent.parent.gameObject.activeSelf) return;

        dialogueText.transform.parent.parent.GetComponent<Animator>().SetTrigger("end");
    }
}
