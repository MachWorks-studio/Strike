using System.Collections;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager Instance { get; private set; }
    
    [SerializeField] private AudioClip roundStartAudioClip;
    [SerializeField] private AudioClip roundEndAudioClip;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    private AudioSource audioSourceTargetManager;
    private Target[] activeTargets;
    private int targetsRemaining;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        audioSourceTargetManager = gameObject.GetComponent<AudioSource>();
        activeTargets = new Target[spawnPoints.Length];
        SpawnAllTargets();
    }

    void SpawnAllTargets()
    {
        if (GameManager.Instance.roundsElapsed > 0)
        {
            audioSourceTargetManager.PlayOneShot(roundStartAudioClip);
        }
        GameManager.Instance.NextRound();
        GameManager.Instance.SetRoundState(ERounds.Running);
        
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
        GameManager.Instance.CompleteRound();
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
