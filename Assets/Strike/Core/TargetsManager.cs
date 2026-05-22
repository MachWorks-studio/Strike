using System.Collections;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    [SerializeField] private AudioClip roundStartAudioClip;
    [SerializeField] private AudioClip roundEndAudioClip;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    private AudioSource audioSourceTargetManager;
    private Target[] activeTargets;
    private int targetsRemaining;
    private int roundsElapsed;

    void Start()
    {
        audioSourceTargetManager = gameObject.GetComponent<AudioSource>();
        activeTargets = new Target[spawnPoints.Length];
        SpawnAllTargets();
    }

    void SpawnAllTargets()
    {
        if (roundsElapsed > 0)
        {
            audioSourceTargetManager.PlayOneShot(roundStartAudioClip);
        }
        roundsElapsed++;
        
        int i = 0;
        foreach (Transform spawnPoint in spawnPoints)
        {
            activeTargets[i] = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Target>();
            activeTargets[i].OnTargetHit += HandleTargetHit;
            i++;
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
        else
        {
            Debug.Log("Target Hit! Manager Knows.");
        }
    }

    void AllTargetsHit()
    {
        audioSourceTargetManager.PlayOneShot(roundEndAudioClip);
        StartCoroutine(SpawnAfterDelay(1f));
        Debug.Log("All Targets Hit, Respawning Targets");
    }
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnAllTargets();
    }
}
