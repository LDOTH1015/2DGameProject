using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConversationUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject FigthButton;
    [SerializeField] private GameObject PrayButton;
    [SerializeField] private GameObject select3in1;

    private Queue<string> sentences = new Queue<string>();
    private bool isDialogueActive = false;

    void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(string[] lines)
    {
        dialoguePanel.SetActive(true);
        sentences.Clear();

        if (sentences.Count != 0) 
        {
            foreach (string line in lines)
            {
                sentences.Enqueue(line);
            }
        }
        isDialogueActive = true;
        DisplayNextSentence();
    }

    void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        dialogueText.gameObject.SetActive(false);
        FigthButton.SetActive(true);
        PrayButton.SetActive(true);
        //dialoguePanel.SetActive(false);
        //isDialogueActive = false;
    }

    public void OnClickFight()
    {

    }

    public void OnClickPray()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        select3in1.SetActive(true);
        
    }
}
