using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NarratorWarning : MonoBehaviour
{
    private bool _saidWarning = false;

    void OnTriggerEnter(Collider other)
    {
        if (!_saidWarning && other.CompareTag("Player"))
        {
            _saidWarning = true;
            GetComponent<AudioSource>().Play();
        }
    }
}