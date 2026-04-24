using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    public string loseCinematicSceneName = "LoseCinematic";
    private bool gameOverTriggered = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoseByGuard()
    {
        if (gameOverTriggered) return;

        gameOverTriggered = true;
        SceneManager.LoadScene(loseCinematicSceneName);
    }
}