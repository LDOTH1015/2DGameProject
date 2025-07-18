using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConversationUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject SelectButtons;
    public PlayerInput playerInput;

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
        SelectButtons.SetActive(true);
        //dialoguePanel.SetActive(false);
        //isDialogueActive = false;
    }

    public void OnClickFight()
    {
        playerInput.enabled = true;
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }

    public void OnClickPray()
    {
        playerInput.enabled = true;
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }

    public void OnClickBack()
    {
        playerInput.enabled = true;
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}
