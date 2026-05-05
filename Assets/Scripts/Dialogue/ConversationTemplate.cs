using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Line
{
    public string speakerName;
    [TextArea(2, 3)]
    public string dialogue;
}

[CreateAssetMenu(fileName = "NewConversation", menuName = "Dialogue/Conversation")]

public class ConversationTemplate : ScriptableObject
{
    public List<Line> conversationLines;
}
