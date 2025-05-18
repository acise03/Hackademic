using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioClip musicClip;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.volume = 0.5f; // Set volume
            audioSource.playOnAwake = false;

            if (musicClip != null)
            {
                audioSource.clip = musicClip;
                audioSource.Play();
            }
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}
