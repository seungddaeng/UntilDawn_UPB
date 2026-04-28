using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FlashlightSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject flashlightVisual;
    public Light flashlightLight;
    public AudioSource clickAudio;

    [Header("State")]
    public bool hasFlashlight = false;
    public bool hasBattery = false;
    public bool flashlightOn = false;

    [Header("Win")]
    public string winCinematicName = "WinCinematic";
    private bool alreadyWon = false;

    private void Start()
    {
        hasFlashlight = false;
        hasBattery = false;
        flashlightOn = false;
        UpdateFlashlightState();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryToggleFlashlight();
        }
    }

    public void GiveFlashlight()
    {
        Debug.Log("Linterna recogida");
        hasFlashlight = true;
        flashlightOn = false;
        UpdateFlashlightState();
    }

    public void GiveBattery()
    {
        Debug.Log("Batería recogida");
        hasBattery = true;
    }

    void TryToggleFlashlight()
    {
        Debug.Log("Intentando usar linterna");
        Debug.Log("hasFlashlight: " + hasFlashlight);
        Debug.Log("hasBattery: " + hasBattery);

        if (!hasFlashlight)
            return;

        if (!hasBattery)
        {
            UIMessageManager.Instance?.ShowMessage("¡Sin batería! Consigue baterías", 2.5f);
            return;
        }

        flashlightOn = !flashlightOn;
        UpdateFlashlightState();

        Debug.Log("flashlightOn: " + flashlightOn);

        if (clickAudio != null)
            clickAudio.Play();

        if (flashlightOn && !alreadyWon)
        {
            alreadyWon = true;
            SceneManager.LoadScene(winCinematicName);
        }
    }

    void UpdateFlashlightState()
    {
        if (flashlightVisual != null)
        {
            flashlightVisual.SetActive(hasFlashlight);
        }

        if (flashlightLight != null)
        {
            bool shouldLightBeOn = hasFlashlight && flashlightOn && hasBattery;

            flashlightLight.gameObject.SetActive(shouldLightBeOn);
            flashlightLight.enabled = shouldLightBeOn;
        }
    }
}
