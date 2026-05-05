using UnityEngine;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private List<ConversationTemplate> conversations;

    private int currentConversationIndex = 0;

    public void Interact()
    {
        if (!dialogueManager.IsDialogueActive)
        {
            StartConversation();
        }
        else
        {
            dialogueManager.NextLine();
        }
    }

    void StartConversation()
    {
        if (conversations.Count == 0) return;

        dialogueManager.GetConversation(conversations[currentConversationIndex]);
        currentConversationIndex++;

        if (currentConversationIndex >= conversations.Count)
        {
            currentConversationIndex = conversations.Count - 1;
        }
    }

    public void StartConversationByIndex(int index)
    {
        if (dialogueManager.IsDialogueActive) return;
        if (index < 0 || index >= conversations.Count) return;

        dialogueManager.GetConversation(conversations[index]);
    }
}