using System.Collections;
using UnityEngine;

public class Torch : MonoBehaviour
{
    private Light torchLight;

    void Awake()
    {
        torchLight = GetComponentInChildren<Light>();

        if (torchLight != null) StartCoroutine(LightFlicker());
    }

    IEnumerator LightFlicker()
    {
        float transitionDuration = 0.2f;
        float changeInterval = 0.5f;

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
}