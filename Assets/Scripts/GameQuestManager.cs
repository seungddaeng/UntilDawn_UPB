using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameQuestManager : MonoBehaviour
{
    public static GameQuestManager Instance;

    public NPCDialogue marceloDialogue;

    private bool finalChaseStarted = false;

    private float lastMarceloInteractionTime = -999f;
    public float marceloInteractionCooldown = 1f;

    public enum QuestStep
    {
        FindMarceloFirst,
        FindFlashlight,
        FindBatteries,
        ReturnToMarceloSecond,
        TryAlexisDoor,
        ReturnToMarceloThird,
        FindKeys,
        GoToAlexisOffice,
        SearchExam,
        CorrectExam,
        ReturnKeyToLibrary,
        EscapeCampus,
        Win
    }

    [Header("Estado actual")]
    public QuestStep currentStep = QuestStep.FindMarceloFirst;

    [Header("Flecha guía")]
    public GuideArrowUI guideArrow;

    [Header("Puntos importantes")]
    public Transform marceloTarget;
    public Transform flashlightTarget;
    public Transform[] batteryTargets;
    public Transform alexisOfficeTarget;
    public Transform keysTarget;
    public Transform examTarget;
    public Transform libraryReturnTarget;
    public Transform campusExitTarget;

    [Header("Marcelo")]
    public GameObject marceloObject;

    [Header("Alexis")]
    public Alexis alexis;
    public Transform alexisSpawnPoint;

    [Header("Guardias")]
    public GuardPatrol[] guards;
    public float finalChaseSpeed = 7f;

    [Header("Escenas")]
    public string winCinematicSceneName = "WinCinematic";

    [Header("Estado del jugador")]
    public bool hasKeys = false;
    public bool hasExam = false;
    public bool examCorrected = false;

    [Header("Baterías")]
    public int requiredBatteries = 3;
    public int batteriesCollected = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartMission();
    }

    private void Update()
    {
        if (currentStep == QuestStep.CorrectExam)
        {
            if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
            {
                CorrectExam();
            }
        }
    }

    private void StartMission()
    {
        currentStep = QuestStep.FindMarceloFirst;

        hasKeys = false;
        hasExam = false;
        examCorrected = false;
        finalChaseStarted = false;
        batteriesCollected = 0;

        ShowMessage("¡Que no te atrapen! Sigue la flecha y busca al Inge Marcelo.");
        SetArrowTarget(marceloTarget);
    }

    public void OnMarceloInteracted()
    {
        if (Time.time - lastMarceloInteractionTime < marceloInteractionCooldown)
        {
            return;
        }

        lastMarceloInteractionTime = Time.time;

        if (currentStep == QuestStep.FindMarceloFirst)
        {
            currentStep = QuestStep.FindFlashlight;

            marceloDialogue.StartConversationByIndex(0);
            SetArrowTarget(flashlightTarget);
            return;
        }

        if (currentStep == QuestStep.ReturnToMarceloSecond)
        {
            currentStep = QuestStep.TryAlexisDoor;

            marceloDialogue.StartConversationByIndex(1);
            SetArrowTarget(alexisOfficeTarget);
            return;
        }

        if (currentStep == QuestStep.ReturnToMarceloThird)
        {
            currentStep = QuestStep.FindKeys;


            marceloDialogue.StartConversationByIndex(2); 
            SetArrowTarget(keysTarget);

            marceloObject.GetComponent<NPCInteractable>().MarkToDisappearAfterDialogue();

            return;
        }

        ShowMessage("Marcelo ya te dijo todo lo que sabía. Ahora sigue la flecha.");
    }

    public void OnFlashlightCollected()
    {
        if (currentStep != QuestStep.FindFlashlight)
        {
            return;
        }

        currentStep = QuestStep.FindBatteries;
        batteriesCollected = 0;

        ShowMessage("Linterna conseguida. Ahora busca las 3 baterías.", 2.5f);
        PointToNextBattery();
    }

    public void OnBatteryCollected()
    {
        if (currentStep != QuestStep.FindBatteries)
        {
            return;
        }

        batteriesCollected++;

        if (batteriesCollected < requiredBatteries)
        {
            ShowMessage("Batería " + batteriesCollected + " conseguida. Busca la siguiente batería.", 2f);
            PointToNextBattery();
            return;
        }

        currentStep = QuestStep.ReturnToMarceloSecond;

        ShowMessage("Baterías conseguidas. Vuelve con el Inge Marcelo.", 2.5f);
        SetArrowTarget(marceloTarget);
    }

    private void PointToNextBattery()
    {
        if (batteryTargets == null || batteryTargets.Length == 0)
        {
            SetArrowTarget(null);
            return;
        }

        if (batteriesCollected >= 0 && batteriesCollected < batteryTargets.Length)
        {
            SetArrowTarget(batteryTargets[batteriesCollected]);
        }
        else
        {
            SetArrowTarget(null);
        }
    }

    public void OnTriedFlashlightWithoutBattery()
    {
        if (currentStep == QuestStep.FindBatteries)
        {
            ShowMessage("Sin batería. Busca las baterías antes de continuar.");
            PointToNextBattery();
        }
        else
        {
            ShowMessage("Sin batería.");
        }
    }

    public void OnAlexisDoorLocked()
    {
        if (currentStep == QuestStep.TryAlexisDoor)
        {
            currentStep = QuestStep.ReturnToMarceloThird;

            ShowMessage("No tienes la llave de Alexis, busca al Inge Marcelo para que te ayude.");
            SetArrowTarget(marceloTarget);
            return;
        }

        ShowMessage("No tienes la llave de Alexis.");
    }

    public void OnKeysCollected()
    {
        if (currentStep != QuestStep.FindKeys)
        {
            return;
        }

        hasKeys = true;
        currentStep = QuestStep.GoToAlexisOffice;

        ShowMessage("Llaves conseguidas, ve a la oficina de Alexis.");
        SetArrowTarget(alexisOfficeTarget);
    }

    public void OnEnteredAlexisOffice()
    {
        if (currentStep != QuestStep.GoToAlexisOffice)
        {
            return;
        }

        currentStep = QuestStep.SearchExam;

        ShowMessage("Busca tu examen y salva la materia. Pero cuidado, Alexis acaba de llegar a la universidad, que no te vea.");
        SetArrowTarget(examTarget);

        if (alexis != null)
        {
            if (alexisSpawnPoint != null)
            {
                alexis.transform.position = alexisSpawnPoint.position;
            }

            alexis.AppearAndStartRoute();
        }

        if (StoryTimeManager.Instance != null)
        {
            StoryTimeManager.Instance.TriggerDawn();
        }
    }

    public void OnExamCollected()
    {
        if (currentStep != QuestStep.SearchExam)
        {
            return;
        }

        hasExam = true;
        currentStep = QuestStep.CorrectExam;

        ShowMessage("Examen conseguido, corrígelo. Presiona G para corregir.");
        SetArrowTarget(null);
    }

    private void CorrectExam()
    {
        examCorrected = true;
        currentStep = QuestStep.ReturnKeyToLibrary;

        ShowMessage("Examen corregido. Devuelve la llave a la biblioteca, de otra manera todo esto habrá sido en vano.");
        SetArrowTarget(libraryReturnTarget);
    }

    public void OnReturnedKeyAndExitedLibrary()
    {
        if (currentStep != QuestStep.ReturnKeyToLibrary)
        {
            return;
        }

        hasKeys = false;
        currentStep = QuestStep.EscapeCampus;

        ShowMessage("Llaves devueltas. ¡Que no te vean! Corre, sal del campus.", 3f);
        SetArrowTarget(campusExitTarget);

        StartFinalChase();
    }

    private void StartFinalChase()
    {
        if (finalChaseStarted)
        {
            return;
        }

        finalChaseStarted = true;

        ShowMessage("¡Tú puedes! ¡¡¡CORRE!!!");

        if (guards == null)
        {
            return;
        }

        foreach (GuardPatrol guard in guards)
        {
            if (guard != null)
            {
                guard.ForceFinalChase(finalChaseSpeed);
            }
        }
    }

    public void OnReachedCampusExit()
    {
        if (currentStep != QuestStep.EscapeCampus)
        {
            return;
        }

        currentStep = QuestStep.Win;
        SceneManager.LoadScene(winCinematicSceneName);
    }

    public void PointToMarcelo()
    {
        SetArrowTarget(marceloTarget);
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
        ShowMessage(message, 4f);
    }

    private void ShowMessage(string message, float duration)
    {
        if (guideArrow != null)
        {
            guideArrow.HideTemporarily(duration);
        }

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ShowMessage(message, duration);
            UIMessageManager.Instance.ClearBottomInstruction();
        }
    }
}