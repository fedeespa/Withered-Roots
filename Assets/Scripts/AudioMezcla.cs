using UnityEngine;
using UnityEngine.Audio;

public class AudioMezcla : MonoBehaviour
{
    
    [SerializeField] private AudioMixerSnapshot Normal;

    [SerializeField] private AudioMixerSnapshot CercaCueva;

    [SerializeField] private AudioMixerSnapshot Cueva;
    [SerializeField] private AudioMixerSnapshot Pantano;

    [Min(0.0f)] public float transitionTime = 1.0f;
    public void normal() { if (Normal != null) Normal.TransitionTo(transitionTime); }
    public void cercacueva() { if (CercaCueva != null) CercaCueva.TransitionTo(transitionTime); } 
    public void cueva() { if (Cueva != null) Cueva.TransitionTo(transitionTime); }
    public void pantano() { if (Pantano != null) Pantano.TransitionTo(transitionTime); }
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
        normal();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            cercacueva();
        }
    }
    

}
