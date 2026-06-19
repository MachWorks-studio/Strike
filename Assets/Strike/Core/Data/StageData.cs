using UnityEngine;

public enum TargetSelectionMode { Random, Specific }

[System.Serializable]
public class RoundTargetData
{
    public TargetSelectionMode targetSelectionMode = TargetSelectionMode.Random;
    
    // Used when mode == Specific
    public int[] specificTargetIndices;
    
    // Used when mode == Random (falls back to targetsCountAtRound if you want)
    public int randomTargetCount;

    public GameObject coverMeshObject;
}
[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    // Element 0 of targetsCountAtRound if for infinite rounds
    public bool infiniteRounds = false;
    
    // Element 0 is for infinite rounds, mirrors targetCountsAtRound indexing
    public RoundTargetData[] roundTargetData;
}
