using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Referencias de linterna")]
    public GameObject flashlightVisual;
    public GameObject flashlightLight;

    [Header("UI de batería")]
    public BatteryUI batteryUI;

    [Header("Audio")]
    public AudioSource flashlightClickAudio;

    [Header("Configuración de batería")]
    public int maxBatteries = 3;
    public int currentBatteries = 0;
    public float secondsPerBattery = 10f;

    private bool hasFlashlight = false;
    private bool flashlightOn = false;
    private float batteryTimer = 0f;
    private Light flashlightLightComponent;

    private void Awake()
    {
        if (flashlightLight != null)
        {
            flashlightLightComponent = flashlightLight.GetComponent<Light>();

            if (flashlightLightComponent == null)
            {
                flashlightLightComponent = flashlightLight.GetComponentInChildren<Light>();
            }
        }
    }

    private void Start()
    {
        currentBatteries = Mathf.Clamp(currentBatteries, 0, maxBatteries);
        hasFlashlight = false;
        flashlightOn = false;
        batteryTimer = 0f;

        UpdateFlashlightState();
        UpdateBatteryUI();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryToggleFlashlight();
        }

        if (flashlightOn && hasFlashlight && currentBatteries > 0)
        {
            DrainBattery();
        }
    }

    public void GiveFlashlight()
    {
        hasFlashlight = true;
        flashlightOn = false;
        batteryTimer = 0f;

        UpdateFlashlightState();

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ShowMessage("Linterna conseguida. Busca las 3 baterías.", 3f);
            UIMessageManager.Instance.SetBottomInstruction("Busca las 3 baterías para cargar la linterna.");
        }
    }

    public void GiveBattery()
    {
        if (currentBatteries >= maxBatteries)
        {
            if (UIMessageManager.Instance != null)
            {
                UIMessageManager.Instance.ShowMessage("La linterna ya tiene la carga máxima.", 2f);
            }

            return;
        }

        currentBatteries++;
        batteryTimer = 0f;

        UpdateBatteryUI();

        if (UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.ShowMessage("Batería conseguida: " + currentBatteries + "/" + maxBatteries, 2f);
        }

        if (currentBatteries == maxBatteries && UIMessageManager.Instance != null)
        {
            UIMessageManager.Instance.SetBottomInstruction("Linterna cargada. Usa click izquierdo para encenderla.");
        }
    }

    private void TryToggleFlashlight()
    {
        if (!hasFlashlight)
        {
            return;
        }

        if (currentBatteries <= 0)
        {
            flashlightOn = false;
            batteryTimer = 0f;
            UpdateFlashlightState();

            if (UIMessageManager.Instance != null)
            {
                UIMessageManager.Instance.ShowMessage("Sin batería. Busca baterías para usar la linterna.", 2f);
            }

            return;
        }

        flashlightOn = !flashlightOn;
        batteryTimer = 0f;

        if (flashlightClickAudio != null)
        {
            flashlightClickAudio.Play();
        }

        UpdateFlashlightState();
    }

    private void DrainBattery()
    {
        batteryTimer += Time.deltaTime;

        if (batteryTimer < secondsPerBattery)
        {
            return;
        }

        batteryTimer = 0f;
        currentBatteries--;
        currentBatteries = Mathf.Clamp(currentBatteries, 0, maxBatteries);

        UpdateBatteryUI();

        if (currentBatteries <= 0)
        {
            flashlightOn = false;
            UpdateFlashlightState();

            if (UIMessageManager.Instance != null)
            {
                UIMessageManager.Instance.ShowMessage("La linterna se quedó sin batería.", 2f);
            }
        }
    }

    private void UpdateFlashlightState()
    {
        bool shouldShowVisual = hasFlashlight;
        bool shouldTurnLightOn = hasFlashlight && flashlightOn && currentBatteries > 0;

        if (flashlightVisual != null)
        {
            flashlightVisual.SetActive(shouldShowVisual);
        }

        if (flashlightLight != null)
        {
            flashlightLight.SetActive(shouldTurnLightOn);
        }

        if (flashlightLightComponent != null)
        {
            flashlightLightComponent.enabled = shouldTurnLightOn;
        }
    }

    private void UpdateBatteryUI()
    {
        if (batteryUI != null)
        {
            batteryUI.UpdateBatteryUI(currentBatteries);
        }
    }
}