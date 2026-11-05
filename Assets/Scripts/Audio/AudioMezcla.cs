using UnityEngine;
using UnityEngine.Audio;

public class AudioMezcla : MonoBehaviour
{

    [SerializeField] private AudioMixerSnapshot normal;

    [SerializeField] private AudioMixerSnapshot cercaCueva;

    [SerializeField] private AudioMixerSnapshot cueva;
    [SerializeField] private AudioMixerSnapshot pantano;

    [Min(0.0f)] public float _transitionTime = 1.0f;
    public void Normal(float? transitionTime = null) { if (normal != null) normal.TransitionTo(transitionTime ?? _transitionTime); }
    public void CercaCueva(float? transitionTime = null) { if (cercaCueva != null) cercaCueva.TransitionTo(transitionTime ?? _transitionTime); }
    public void Cueva(float? transitionTime = null) { if (cueva != null) cueva.TransitionTo(transitionTime ?? 0f); }
    public void Pantano(float? transitionTime = null) { if (pantano != null) pantano.TransitionTo(transitionTime ?? _transitionTime); }
    private static AudioMezcla instance;
    public static AudioMezcla Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
