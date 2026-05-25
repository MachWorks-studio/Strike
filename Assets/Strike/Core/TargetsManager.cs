using System.Collections;
using System.Linq;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager Instance { get; private set; }
    
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int targetCount = 5;
    
    [SerializeField] Target[] activeTargets;
    public int targetsRemaining;
    public bool willSpawnNextRound = true;
    
    private Coroutine _waitForNextRoundCoroutine;

    //For Clamping Inspector values
    private void OnValidate()
    {
        targetCount = Mathf.Clamp(targetCount, 1, spawnPoints.Length);
    }
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
        //SpawnAllTargets();
    }

    public void SpawnAllTargets()
    {
        int spawnCount = Mathf.Clamp(targetCount, 0, spawnPoints.Length);
        
        Transform[] shuffledTargets = spawnPoints.OrderBy(_=> Random.value).ToArray();
        
        activeTargets = new Target[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            activeTargets[i] = Instantiate(targetPrefab, shuffledTargets[i].position, shuffledTargets[i].rotation).GetComponent<Target>();
            activeTargets[i].OnTargetHit += HandleTargetHit;
        }
        targetsRemaining = activeTargets.Length;
    }

    public void DespawnAllTargets()
    {
        if (activeTargets != null)
        {
            foreach (Target activeTarget in activeTargets)
            {
                if (activeTarget != null)
                {
                    activeTarget.OnTargetHit -= HandleTargetHit;
                    Destroy(activeTarget.gameObject);
                }
            }
        }
        activeTargets = null;
        targetsRemaining = 0;
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
        activeTargets = null;
        GameManager.Instance.EndRound();
        if (willSpawnNextRound)
        {
            _waitForNextRoundCoroutine = StartCoroutine(SpawnAfterDelay(2f));
        }
        else
        {
            
        }
    }
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.NextRound();
    }

    public void SetContinueToNextRound(bool value)
    {
        willSpawnNextRound = value;
    }

    public void StopWaitingForNextRound()
    {
        if (_waitForNextRoundCoroutine != null)
        {
            StopCoroutine(_waitForNextRoundCoroutine);
            _waitForNextRoundCoroutine = null;
        }
    }
}
