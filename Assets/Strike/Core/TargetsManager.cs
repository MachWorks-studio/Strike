using System.Collections;
using System.Linq;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager Instance { get; private set; }
    [SerializeField] private RoundTargetData[] currentRoundTargetData;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] public int targetCount = 5;
    
    [SerializeField] Target[] activeTargets;
    public int targetsRemaining;
    public bool willSpawnNextRound = true;
    
    private Coroutine _waitForNextRoundCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitializeRoundTargetData(RoundTargetData[] data)
    {
        currentRoundTargetData = data;
    }

    public void UpdateTargetCount(int roundNumber)
    {
        RoundTargetData roundData = currentRoundTargetData[roundNumber];

        if (roundData.targetSelectionMode == TargetSelectionMode.Specific)
        {
            targetCount = roundData.specificTargetIndices.Length;
        }
        else
        {
            targetCount = roundData.randomTargetCount;
        }
    }
    public void SpawnAllTargets(int roundNumber)
    {
        RoundTargetData roundData = currentRoundTargetData[roundNumber];

        int[] indicesToSpawn;
        if (roundData.targetSelectionMode == TargetSelectionMode.Specific)
        {
            indicesToSpawn = roundData.specificTargetIndices;
        }
        else
        {
            indicesToSpawn = spawnPoints
                .Select((point, index) => index)
                .OrderBy(_ => Random.value)
                .Take(targetCount)
                .ToArray();
        }

        activeTargets = new Target[indicesToSpawn.Length];

        for (int i = 0; i < indicesToSpawn.Length; i++)
        {
            Transform spawnPoint = spawnPoints[indicesToSpawn[i]];
            activeTargets[i] = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Target>();
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
            _waitForNextRoundCoroutine = StartCoroutine(SpawnAfterDelay(2f));
    }
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.NextRound();
    }

    public void SetContinueToNextRound(bool value) => willSpawnNextRound = value;

    public void StopWaitingForNextRound()
    {
        if (_waitForNextRoundCoroutine != null)
        {
            StopCoroutine(_waitForNextRoundCoroutine);
            _waitForNextRoundCoroutine = null;
        }
    }
}
