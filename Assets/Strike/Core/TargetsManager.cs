using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
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

#if UNITY_EDITOR
    private List<GameObject> _spawnLabels = new List<GameObject>();
    void OnEnable()
    {
        ShowTargetNumber();
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    void OnDisable()
    {
        ClearSpawnLabels();
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            ClearSpawnLabels();
    }

    private void ShowTargetNumber()
    {
        if (Application.isPlaying) return;
        ClearSpawnLabels();
        if (spawnPoints == null) return;
        
        GameObject labelParent = new GameObject("---SPAWN LABELS---");
        labelParent.hideFlags = HideFlags.DontSave;
        _spawnLabels.Add(labelParent);
        
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject labelObj = new GameObject("__SpawnLabel_" + i);
            labelObj.hideFlags = HideFlags.DontSave;
            labelObj.transform.SetParent(labelParent.transform);
            labelObj.transform.position = spawnPoints[i].position + Vector3.up * 0.2f;

            TextMeshPro tmp = labelObj.AddComponent<TextMeshPro>();
            tmp.font = TMP_Settings.defaultFontAsset;
            tmp.text = i.ToString();
            tmp.fontSize = 3f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.rectTransform.sizeDelta = new Vector2(5f, 5f);
            
            _spawnLabels.Add(labelObj);
        }
    }

    private void ClearSpawnLabels()
    {
        foreach (var label in _spawnLabels)
        {
            if (label != null)
                DestroyImmediate(label);
        }

        _spawnLabels.Clear();
        
        // For deleting target labels by name
        var orphans = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
            .Where(go => go.name.StartsWith("__SpawnLabel_"));
        foreach (var orphan in orphans)
            DestroyImmediate(orphan);
    }
#endif
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        #if UNITY_EDITOR
            if (Application.isPlaying)
                ClearSpawnLabels();
        #endif
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
