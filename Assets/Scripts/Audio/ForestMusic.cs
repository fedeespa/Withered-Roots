using System.Collections;
using UnityEngine;

public class ForestMusic : MonoBehaviour
{
    [SerializeField] private AudioSource _otherAudioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Crossfade());
        }
    }

    public IEnumerator Crossfade()
    {
        AudioSource audioSource = GetComponent<AudioSource>(); 

        float startFromVol = _otherAudioSource.volume;

        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0f;
            audioSource.Play();
        }
        float startToVol = audioSource.volume;

        float t = 0f;
        while (t < 5f)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / 5f);

            _otherAudioSource.volume = Mathf.Lerp(startFromVol, 0f, k);
            audioSource.volume = Mathf.Lerp(startToVol, 1f, k);

            yield return null;
        }

        _otherAudioSource.volume = 0f;
        audioSource.volume = 1f;

        if (_otherAudioSource.isPlaying) _otherAudioSource.Stop();
    }
}