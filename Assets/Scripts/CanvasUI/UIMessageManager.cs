using TMPro;
using UnityEngine;
using System.Collections;

public class UIMessageManager : MonoBehaviour
{
    public static UIMessageManager Instance;

    public TextMeshProUGUI centerMessageText;
    public TextMeshProUGUI bottomInstructionText;

    private Coroutine centerMessageRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowMessage(string message, float duration = 2f)
    {
        if (centerMessageRoutine != null)
            StopCoroutine(centerMessageRoutine);

        centerMessageRoutine = StartCoroutine(ShowMessageRoutine(message, duration));
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        if (centerMessageText != null)
            centerMessageText.text = message;

        yield return new WaitForSeconds(duration);

        if (centerMessageText != null)
            centerMessageText.text = "";
    }

    public void SetBottomInstruction(string message)
    {
        if (bottomInstructionText != null)
            bottomInstructionText.text = message;
    }

    public void ClearBottomInstruction()
    {
        if (bottomInstructionText != null)
            bottomInstructionText.text = "";
    }
}