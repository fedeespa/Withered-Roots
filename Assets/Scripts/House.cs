using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class House : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public void Open()
    {
        _audioSource.Stop();
        _audioSource.Play();
    }
}