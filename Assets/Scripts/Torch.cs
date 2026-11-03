using System.Collections;
using UnityEngine;

public class Torch : MonoBehaviour
{
    private Light torchLight;

    void Awake()
    {
        torchLight = GetComponentInChildren<Light>();

        if (torchLight != null) StartCoroutine(LightFlicker());
        StartCoroutine(SparkRoutine());
    }

    IEnumerator LightFlicker()
    {
        float transitionDuration = 0.2f;

        while (true)
        {
            float targetIntensity = Random.Range(30, 50);
            float targetGreen = Random.Range(150, 170);

            float startIntensity = torchLight.intensity;
            Color startColor = torchLight.color;

            float t = 0f;
            while (t < transitionDuration)
            {
                t += Time.deltaTime;
                torchLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t / transitionDuration);
                torchLight.color = new Color32(255, (byte)Mathf.Lerp(startColor.g * 255f, targetGreen, t / transitionDuration), 0, 255);
                yield return null;
            }
            torchLight.intensity = targetIntensity;
            torchLight.color = new Color32(255, (byte)targetGreen, 0, 255);

            yield return null;
        }
    }

    private IEnumerator SparkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1.0f, 0.05f));

            PlaySpark();
        }
    }

    private void PlaySpark()
    {
        AudioSource audioSource = GetComponentInChildren<AudioSource>();
        AudioClip clip = audioSource.clip;
        if (clip == null) return;

        float volume = Random.Range(0.2f, 1f);
        float pitch = Random.Range(0.5f, 1.5f);

        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume);
    }
}