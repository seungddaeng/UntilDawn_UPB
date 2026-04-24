using UnityEngine;
using UnityEngine.SceneManagement;

// Este script administra toda la musica del juego:
// menu, niveles, derrota y victoria.
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip gameOverMusic;
    public AudioClip winMusic;

    private AudioSource audioSource;

    private void Awake()
    {
        // Esto lo uso para que no se dupliquen managers de musica entre escenas.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Si el objeto ya tiene AudioSource lo uso,
        // si no, se lo agrego.
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    // Apenas empieza, reviso en que escena estoy y pongo la musica correcta.
    private void Start()
    {
        ChangeMusicForScene(SceneManager.GetActiveScene().name);
    }

    // Me suscribo al cambio de escena para actualizar la musica.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Me desuscribo si el objeto se desactiva.
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Cada vez que cambia la escena, cambio la musica.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeMusicForScene(scene.name);
    }

    // Decidir que musica va en cada escena.
    private void ChangeMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                PlayMusic(menuMusic, true);
                break;

            case "Level1":
            case "Level2":
            case "Level3":
                PlayMusic(levelMusic, true);
                break;

            case "GameOver":
                PlayMusic(gameOverMusic, false);
                break;

            case "WinScreen":
                PlayMusic(winMusic, false);
                break;
        }
    }

    // Reproducir el clip que corresponda y decidir si debe repetirse o no.
    private void PlayMusic(AudioClip clip, bool loop)
    {
        if (clip == null) return;

        audioSource.loop = loop;

        if (audioSource.clip != clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}