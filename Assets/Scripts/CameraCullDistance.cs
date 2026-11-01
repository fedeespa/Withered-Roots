using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraCullDistance : MonoBehaviour
{
    [SerializeField] private float _cullDistance;

    void Start()
    {
        float[] distances = new float[32];
        distances[LayerMask.NameToLayer("Grass")] = _cullDistance;
        GetComponent<Camera>().layerCullDistances = distances;
    }
}