using System.Collections;
using UnityEngine;

public class ForestMusic : MonoBehaviour
{
    [SerializeField] private LevelMusicState _levelMusicState;
    [SerializeField] private AudioSource _otherAudioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_levelMusicState != LevelMusicState.nearCave)
            {
                StartCoroutine(Crossfade());
            }

            switch (_levelMusicState)
            {
                case LevelMusicState.normal:
                    {
                        AudioMezcla.Instance.Normal();
                        break;
                    }
                case LevelMusicState.swamp:
                    {
                        AudioMezcla.Instance.Pantano();
                        break;
                    }
                case LevelMusicState.nearCave:
                    {
                        AudioMezcla.Instance.CercaCueva();
                        break;
                    }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _levelMusicState == LevelMusicState.nearCave)
        {
            AudioMezcla.Instance.Pantano();
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

public enum LevelMusicState
{
    normal, swamp, nearCave
}