using UnityEngine;
using System.Collections;

public class StoryTimeManager : MonoBehaviour
{
    public static StoryTimeManager Instance;

    public Light directionalLight;

    public Material eveningSkybox;
    public Material nightSkybox;
    public Material dawnSkybox;

    public Color eveningLightColor = new Color(1f, 0.6f, 0.3f);
    public Color nightLightColor = new Color(0.03f, 0.03f, 0.08f);
    public Color dawnLightColor = new Color(0.8f, 0.75f, 0.65f);

    public Color eveningAmbient = new Color(0.25f, 0.2f, 0.15f);
    public Color nightAmbient = new Color(0.01f, 0.01f, 0.02f);
    public Color dawnAmbient = new Color(0.25f, 0.3f, 0.35f);

    public float transitionDuration = 20f;

    private void Awake()
    {
        Instance = this;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
    }

    private void Start()
    {
        ApplyInstantEvening();
    }

    void ApplyInstantEvening()
    {
        RenderSettings.skybox = eveningSkybox;
        RenderSettings.ambientLight = eveningAmbient;

        directionalLight.color = eveningLightColor;
        directionalLight.intensity = 0.6f;
        directionalLight.transform.rotation = Quaternion.Euler(20f, 30f, 0f);

        DynamicGI.UpdateEnvironment();
    }

    public void TriggerNight()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionTo(
            nightSkybox,
            nightLightColor,
            nightAmbient,
            0.05f,
            Quaternion.Euler(-30f, 0f, 0f)
        ));
    }

    public void TriggerDawn()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionTo(
            dawnSkybox,
            dawnLightColor,
            dawnAmbient,
            0.4f,
            Quaternion.Euler(10f, 60f, 0f)
        ));
    }

    IEnumerator TransitionTo(
    Material targetSkybox,
    Color targetLightColor,
    Color targetAmbient,
    float targetIntensity,
    Quaternion targetRotation
)
    {
        Color startLightColor = directionalLight.color;
        Color startAmbient = RenderSettings.ambientLight;
        float startIntensity = directionalLight.intensity;
        Quaternion startRotation = directionalLight.transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            directionalLight.color =
                Color.Lerp(startLightColor, targetLightColor, smoothT);

            directionalLight.intensity =
                Mathf.Lerp(startIntensity, targetIntensity, smoothT);

            directionalLight.transform.rotation =
                Quaternion.Slerp(startRotation, targetRotation, smoothT);

            RenderSettings.ambientLight =
                Color.Lerp(startAmbient, targetAmbient, smoothT);

            yield return null;
        }

        RenderSettings.skybox = targetSkybox;
        DynamicGI.UpdateEnvironment();
    }
}