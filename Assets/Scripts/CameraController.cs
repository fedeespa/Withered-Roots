using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 target = Vector3.zero;
    public float distance = 15f;
    public float pitch = 35.264f;
    public float yawSensitivity = 0.3f;

    private float currentYaw = 135f;

    void Start() => ApplyTransform();

    public void ApplyTransform(float yawDelta = 0)
    {
        currentYaw += yawDelta * yawSensitivity;
        Quaternion rotation = Quaternion.Euler(pitch, currentYaw, 0f);
        transform.SetPositionAndRotation(target - rotation * Vector3.forward * distance, rotation);
    }
}