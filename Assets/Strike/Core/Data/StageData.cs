using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public int roundsInCircuit = 5;
    // Element 0 of targetsCountAtRound if for infinite rounds
    public int[] targetsCountAtRound = new int[] {5, 5};
    public bool infiniteRounds = false;
}
