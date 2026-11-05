using UnityEngine;
using UnityEngine.Audio;

public class AudioMezcla : MonoBehaviour
{

    [SerializeField] private AudioMixerSnapshot normal;

    [SerializeField] private AudioMixerSnapshot cercaCueva;

    [SerializeField] private AudioMixerSnapshot cueva;
    [SerializeField] private AudioMixerSnapshot pantano;

    [Min(0.0f)] public float transitionTime = 1.0f;
    public void Normal() { if (normal != null) normal.TransitionTo(transitionTime); }
    public void CercaCueva() { if (cercaCueva != null) cercaCueva.TransitionTo(transitionTime); }
    public void Cueva() { if (cueva != null) cueva.TransitionTo(0f); }
    public void Pantano() { if (pantano != null) pantano.TransitionTo(transitionTime); }
    private static AudioMezcla instance;
    public static AudioMezcla Instance => instance;

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
    private void Start()
    {
        normal.TransitionTo(0f);
    }
}
