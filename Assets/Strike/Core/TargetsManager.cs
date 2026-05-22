using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] activeTargets;
    void Awake()
    {
        
    }

    void Start()
    {
        activeTargets = new GameObject[spawnPoints.Length];
        SpawnAllTargets();
    }

    void SpawnAllTargets()
    {
        int i = 0;
        foreach (Transform spawnPoint in spawnPoints)
        {
            activeTargets[i] = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation);
            activeTargets[i].GetComponent<Target>().OnTargetHit += HandleTargetHit;
            i++;
        }
    }

    void HandleTargetHit()
    {
        Debug.Log("Target Hit! Manager Knows.");
    }
}
