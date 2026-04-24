using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private ConversationTemplate conversation;

    public void Interact()
    {
        if (!dialogueManager.IsDialogueActive)
        {
            dialogueManager.GetConversation(conversation);
        }
        else
        {
            dialogueManager.NextLine();
        }
    }
}