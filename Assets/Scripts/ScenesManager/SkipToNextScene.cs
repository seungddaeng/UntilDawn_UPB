using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipToNextScene : MonoBehaviour
{
    public void SkipIntroCinematic()
    {
        SceneManager.LoadScene("PrincipalScene");
    }

    public void SkipWinCinematic()
    {
        SceneManager.LoadScene("WinScreen");
    }

    public void SkipLoseCinematic()
    {
        SceneManager.LoadScene("GameOver");
    }
}
