using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    private static AudioController instance;
    public static AudioController Instance => instance;

    [SerializeField] private AudioClip relaxedClip;
    [SerializeField] private AudioClip worrisomeClip;
    [SerializeField] private AudioClip combatClip;

    private MusicMood _musicMood = MusicMood.relaxed;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        var audioSource = GetComponent<AudioSource>();

        audioSource.clip = AudioClipForMood(_musicMood);
        audioSource.Play();
    }

    AudioClip AudioClipForMood(MusicMood mood)
    {
        return mood switch
        {
            MusicMood.relaxed => relaxedClip,
            MusicMood.worrisome => worrisomeClip,
            MusicMood.combat => combatClip,
            _ => null,
        };
    }
}

public enum MusicMood
{
    relaxed, worrisome, combat,
}