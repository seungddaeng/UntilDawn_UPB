using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject dialoguePanel;

    bool dialogueIsActive = false;
    Queue<Line> lines = new Queue<Line>();

    public bool IsDialogueActive => dialogueIsActive;

    private void Update()
    {

    }
    public void NextLine()
    {
        if (!dialogueIsActive) return;
        ProduceNextLine();
    }

    public void GetConversation(ConversationTemplate conversation)
    {
        lines.Clear();
        foreach (var line in conversation.conversationLines)
        {
            lines.Enqueue(line);
        }

        dialogueIsActive = true;
        dialoguePanel.SetActive(true);
        ProduceNextLine();
    }

    void ProduceNextLine()
    {
        if (lines.Count == 0)
        {
            dialogueIsActive = false;
            dialoguePanel.SetActive(false);
            return;
        }

        Line currentLine = lines.Dequeue();
        nameText.text = currentLine.speakerName;
        dialogueText.text = currentLine.dialogue;
    }
}