using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CircuitSavedData", menuName = "Scriptable Objects/CircuitSavedData")]
public class CircuitSavedData : ScriptableObject
{
    [Header("Circuit Information")]
    public string circuitName;
    public int circuitIndex;
    
    [Header("Circuit Times")]
    public List<float> circuitTimes = new List<float>();

    public void AddTime(float time)
    {
        circuitTimes.Add(time);
        circuitTimes.Sort();
    }
    
    public float BestTime() => circuitTimes.Count > 0 ? circuitTimes[0] : float.MaxValue;
}
