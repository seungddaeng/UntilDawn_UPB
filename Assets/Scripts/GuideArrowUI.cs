using UnityEngine;

public class GuideArrowUI : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform arrowRect;
    public Camera playerCamera;
    public Transform target;

    [Header("Configuración")]
    public float edgePadding = 80f;
    public float hideDistance = 3f;

    private void Start()
    {
        if (arrowRect == null)
        {
            arrowRect = GetComponent<RectTransform>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (target == null || playerCamera == null || arrowRect == null)
        {
            arrowRect.gameObject.SetActive(false);
            return;
        }

        float distance = Vector3.Distance(playerCamera.transform.position, target.position);

        if (distance <= hideDistance)
        {
            arrowRect.gameObject.SetActive(false);
            return;
        }

        arrowRect.gameObject.SetActive(true);

        Vector3 screenPosition = playerCamera.WorldToScreenPoint(target.position);
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        if (screenPosition.z < 0)
        {
            screenPosition *= -1f;
        }

        Vector3 direction = (screenPosition - screenCenter).normalized;

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
    }
}