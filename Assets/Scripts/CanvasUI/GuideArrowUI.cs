using UnityEngine;
using UnityEngine.UI;

public class GuideArrowUI : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform arrowRect;
    public Camera playerCamera;
    public Transform target;

    [Header("Configuración")]
    public float edgePadding = 80f;
    public float hideDistance = 1.2f;

    private Image arrowImage;
    private float hiddenUntil = 0f;

    private void Awake()
    {
        if (arrowRect == null)
        {
            arrowRect = GetComponent<RectTransform>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        arrowImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (arrowRect == null)
        {
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (Time.time < hiddenUntil)
        {
            SetVisible(false);
            return;
        }

        if (target == null || playerCamera == null)
        {
            SetVisible(false);
            return;
        }

        float distance = Vector3.Distance(playerCamera.transform.position, target.position);

        if (distance <= hideDistance)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        Vector3 screenPosition = playerCamera.WorldToScreenPoint(target.position);
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        if (screenPosition.z < 0)
        {
            screenPosition *= -1f;
        }

        Vector3 direction = (screenPosition - screenCenter).normalized;

        if (direction == Vector3.zero)
        {
            SetVisible(false);
            return;
        }

        Vector3 clampedPosition = screenCenter + direction * 220f;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, edgePadding, Screen.width - edgePadding);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, edgePadding, Screen.height - edgePadding);

        arrowRect.position = clampedPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        SetVisible(newTarget != null);
    }

    public void HideTemporarily(float seconds)
    {
        hiddenUntil = Time.time + seconds;
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (arrowImage != null)
        {
            arrowImage.enabled = visible;
        }
    }
}