using UnityEngine;
using UnityEngine.Audio;

public class AudioMezcla : MonoBehaviour
{
    
    [SerializeField] private AudioMixerSnapshot Normal;

    [SerializeField] private AudioMixerSnapshot CercaCueva;

    [Min(0.0f)] public float transitionTime = 1.0f;
    public void normal() { if (Normal != null) Normal.TransitionTo(transitionTime); }
    public void cercacueva() { if (CercaCueva != null) CercaCueva.TransitionTo(transitionTime); } 
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
