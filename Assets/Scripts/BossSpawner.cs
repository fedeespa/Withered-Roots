using System.Collections;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Transform retreatPoint; 
    public Transform RetreatPoint => retreatPoint;
    public static BossSpawner Instance { get; private set; }

    [SerializeField] private GameObject boss;         
    [SerializeField] private AudioSource voiceSource;  
   
    [SerializeField] private float hiddenDepth = 3f;   
    [SerializeField] private float riseDuration = 2.5f;
    [SerializeField] private float afterRiseDelay = 0.5f;

    public bool HasSpawned { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator SpawnBoss()
    {
        HasSpawned = true;

        Vector3 endPos = boss.transform.position;
        Vector3 startPos = endPos + Vector3.down * hiddenDepth;

        boss.transform.position = startPos; // primero la bajo, después la activo
        boss.SetActive(true);

           

        float t = 0f;
        while (t < riseDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / riseDuration);
            boss.transform.position = Vector3.Lerp(startPos, endPos, k);
            yield return null;
        }

        boss.transform.position = endPos; // posición exacta, importa para el pathfinding
        yield return new WaitForSeconds(afterRiseDelay);
    }
}