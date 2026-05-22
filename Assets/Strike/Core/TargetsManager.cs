using System.Collections;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager Instance { get; private set; }
    
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    private Target[] activeTargets;
    [SerializeField] int targetsRemaining;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        activeTargets = new Target[spawnPoints.Length];
    }
    
    void Start()
    {
        //SpawnAllTargets();
    }

    public void SpawnAllTargets()
    {
        int i = 0;
        foreach (Transform spawnPoint in spawnPoints)
        {
            activeTargets[i] = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Target>();
            activeTargets[i].OnTargetHit += HandleTargetHit;
            i++;
        }
        targetsRemaining = activeTargets.Length;
    }

    public void DespawnAllTargets()
    {
        foreach (Target activeTarget in activeTargets)
        {
            Destroy(activeTarget.gameObject);
        }
        targetsRemaining = activeTargets.Length;
    }

    void HandleTargetHit()
    {
        targetsRemaining--;
        if (targetsRemaining <= 0)
        {
            AllTargetsHit();
        }
    }

    void AllTargetsHit()
    {
        GameManager.Instance.EndRound();
        StartCoroutine(SpawnAfterDelay(2f));
    }
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.NextRound();
    }
}
