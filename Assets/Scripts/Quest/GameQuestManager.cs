using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public Transform[] guardRallyPoints;
    public float finalChaseSpeed = 7f;
    public float rallyDelayBeforeChase = 5f;

    [Header("Escenas")]
    public string winCinematicSceneName = "WinCinematic";
    public string loseCinematicSceneName = "LoseCinematic";

    [Header("Estado del jugador")]
    public bool hasKeys = false;
    public bool hasExam = false;
    public bool examCorrected = false;

    [Header("Baterías")]
    public int requiredBatteries = 3;
    public int batteriesCollected = 0;

    [Header("Final Timer")]
    public float escapeTime = 90f;
    public TMP_Text countdownText;
    public GameObject countdownCanvas;

    private float currentTime;
    private bool countdownActive;

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

        if (!countdownActive)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            LoseGame();
            return;
        }

        UpdateCountdownUI();
    }

    private void StartMission()
    {
        currentStep = QuestStep.FindMarceloFirst;

        hasKeys = false;
        hasExam = false;
        examCorrected = false;
        finalChaseStarted = false;
        batteriesCollected = 0;

        ShowMessage("¡Que no te atrapen! Sigue la flecha y busca al Inge Marcelo.", 5f);
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

        StartEscapeCountdown();
    }

    public void OnExamCollected()
    {
        if (currentStep != QuestStep.SearchExam)
        {
            return;
        }

        hasExam = true;
        currentStep = QuestStep.CorrectExam;

        ShowMessage("Examen conseguido, corrígelo. Presiona G para corregir. ¡Cuidado con el tiempo!");
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

        ShowMessage("¡Tú puedes! ¡¡¡CORRE!!!", 3f);

        StartCoroutine(StartRallyThenFinalChase());
    }

    private IEnumerator StartRallyThenFinalChase()
    {
        if (guards == null || guards.Length == 0)
        {
            yield break;
        }

        for (int i = 0; i < guards.Length; i++)
        {
            GuardPatrol guard = guards[i];

            if (guard == null)
            {
                continue;
            }

            Transform rallyPoint = null;

            if (guardRallyPoints != null && i < guardRallyPoints.Length)
            {
                rallyPoint = guardRallyPoints[i];
            }

            guard.MoveToRallyPointThenChase(rallyPoint, finalChaseSpeed);
        }

        yield return new WaitForSeconds(rallyDelayBeforeChase);

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

        countdownActive = false;

        if (countdownCanvas != null)
            countdownCanvas.SetActive(false);

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

    private void StartEscapeCountdown()
    {
        currentTime = escapeTime;
        countdownActive = true;

        if (countdownCanvas != null)
            countdownCanvas.SetActive(true);

        UpdateCountdownUI();
    }

    private void UpdateCountdownUI()
    {
        if (countdownText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        countdownText.text = $"{minutes:00}:{seconds:00}";
    }

    private void LoseGame()
    {
        countdownActive = false;

        if (countdownCanvas != null)
            countdownCanvas.SetActive(false);

        SceneManager.LoadScene(loseCinematicSceneName);
    }
}