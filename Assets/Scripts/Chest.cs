using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject _chestTop;
    [SerializeField] private GameObject _chestBottom;
    [SerializeField] private ParticleSystem _particleSystem;

    public async void Open()
    {
        StartCoroutine(RotateRoutine());
        DisableEmission(_chestTop);
        DisableEmission(_chestBottom);

        await Task.Delay(1000);
        _particleSystem.Stop();
        StartCoroutine(FadeOutCoroutine(_chestTop));
        StartCoroutine(FadeOutCoroutine(_chestBottom));
        StartCoroutine(FadeOutCoroutine(GetComponent<AudioSource>()));
    }

    private void DisableEmission(GameObject gameObject)
    {
        var material = gameObject.GetComponent<Renderer>().material;
        material.DisableKeyword("_EMISSION");

        if (material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", Color.black);
        }
    }

    private IEnumerator RotateRoutine()
    {
        float t = 0f;
        float duration = 0.8f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            float eased = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f).Evaluate(normalized);

            _chestTop.transform.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, new(-90f, 0f, 0f), eased);
            yield return null;
        }

        _chestTop.transform.localEulerAngles = new(-90f, 0f, 0f);
    }

    private IEnumerator FadeOutCoroutine(GameObject gameObject)
    {
        float t = 0f;

        float duration = 5f;
        while (t < duration)
        {
            t += Time.deltaTime;
            gameObject.transform.position = gameObject.transform.position + new Vector3(0, -0.5f * Time.deltaTime, 0);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private IEnumerator FadeOutCoroutine(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        float duration = 5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }
}