using UnityEngine;

public class GameQuestManager : MonoBehaviour
{
    public static GameQuestManager Instance;

    public enum QuestStep
    {
        FindMarcelo,
        FindFlashlight,
        ReturnToMarceloAfterFlashlight,
        FindBatteries,
        ReturnToMarceloAfterBatteries,
        FindKeys,
        GoToAlexisOffice
    }

    [Header("Estado actual")]
    public QuestStep currentStep = QuestStep.FindMarcelo;

    [Header("Referencias de guía")]
    public GuideArrowUI guideArrow;
    public Transform marceloTarget;
    public Transform flashlightTarget;
    public Transform[] batteryTargets;
    public Transform keysTarget;
    public Transform alexisOfficeTarget;

    [Header("Llaves")]
    public bool hasKeys = false;

    private int collectedBatteries = 0;
    private int currentBatteryTargetIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartFindMarcelo();
    }

    private void StartFindMarcelo()
    {
        currentStep = QuestStep.FindMarcelo;

        SetArrowTarget(marceloTarget);

        ShowMessage("¡Que no te atrapen! Debes conseguir tu examen en la oficina de Alexis, pero primero busca a Marcelo.");
    }

    public void OnMarceloReached()
    {
        if (currentStep == QuestStep.FindMarcelo)
        {
            currentStep = QuestStep.FindFlashlight;

            ShowMessage("Marcelo: ¿Qué haces tan tarde? Bueno, te ayudaré, pero no puedes decirle a nadie. Busca la linterna y cuidado con los guardias.");
            SetArrowTarget(flashlightTarget);
            return;
        }

        if (currentStep == QuestStep.ReturnToMarceloAfterFlashlight)
        {
            currentStep = QuestStep.FindBatteries;

            ShowMessage("Marcelo: Genial, encontraste mi linterna. Aunque no recuerdo si tiene baterías... intenta encenderla.");
            SetArrowTarget(marceloTarget);
            return;
        }

        if (currentStep == QuestStep.ReturnToMarceloAfterBatteries)
        {
            currentStep = QuestStep.FindKeys;

            ShowMessage("Marcelo: Bien, ahora busca la llave de la oficina de Alexis. Creo que las guardan en biblioteca. ¡Sé rápido, casi amanece!");
            SetArrowTarget(keysTarget);
            return;
        }
    }

    public void OnFlashlightCollected()
    {
        if (currentStep != QuestStep.FindFlashlight)
        {
            return;
        }

        currentStep = QuestStep.ReturnToMarceloAfterFlashlight;

        ShowMessage("Linterna conseguida. Vuelve con Marcelo.");
        SetArrowTarget(marceloTarget);
    }

    public void OnTriedFlashlightWithoutBattery()
    {
        if (currentStep == QuestStep.ReturnToMarceloAfterFlashlight || currentStep == QuestStep.FindBatteries)
        {
            currentStep = QuestStep.FindBatteries;

            ShowMessage("Marcelo: Es verdad, necesitas baterías. Deberías buscarlas por la universidad, pero cuidado con los guardias.");
            GoToNextBatteryTarget();
        }
    }

    public void OnBatteryCollected()
    {
        collectedBatteries++;

        if (collectedBatteries < 3)
        {
            GoToNextBatteryTarget();
            return;
        }

        currentStep = QuestStep.ReturnToMarceloAfterBatteries;

        ShowMessage("¡Baterías conseguidas! Enciende tu linterna y vuelve con Marcelo.");
        SetArrowTarget(marceloTarget);
    }

    public void OnKeysCollected()
    {
        hasKeys = true;
        currentStep = QuestStep.GoToAlexisOffice;

        ShowMessage("Parece que los guardias escucharon ruido, ¡CORRE!");
        SetArrowTarget(alexisOfficeTarget);
    }

    public void PointToMarcelo()
    {
        SetArrowTarget(marceloTarget);
    }

    private void GoToNextBatteryTarget()
    {
        if (batteryTargets == null || batteryTargets.Length == 0)
        {
            return;
        }

        if (currentBatteryTargetIndex >= batteryTargets.Length)
        {
            currentBatteryTargetIndex = batteryTargets.Length - 1;
        }

        SetArrowTarget(batteryTargets[currentBatteryTargetIndex]);
        currentBatteryTargetIndex++;
    }

    private void SetArrowTarget(Transform target)
    {
        if (guideArrow != null)
        {
            guideArrow.SetTarget(target);
        }
    }

    private void ShowMessage(string message)
    {
        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ShowMessage(message, 5f);
            UIMessageManager.Instance.ClearBottomInstruction();
        }
    }
}