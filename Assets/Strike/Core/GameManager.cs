using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int roundsElapsed;
    [SerializeField] private ERounds currentRoundState;
    [SerializeField] private float currentRoundTime;
    [SerializeField] private float[] previousRoundTimes = Array.Empty<float>();
    private bool roundIsRunning;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (!roundIsRunning) return;
        currentRoundTime += Time.deltaTime;
        
    }
    public void SetRoundState(ERounds state)
    {
        currentRoundState = state;
    }

    public void StartRound()
    {
        SetRoundState(ERounds.Running);
        currentRoundTime = 0f;
        roundIsRunning = true;
    }

    public void CompleteRound()
    {
        roundIsRunning = false;
        SetRoundState(ERounds.Completed);
        SaveRoundTime();
    }
    public void NextRound()
    {
        roundsElapsed++;
        StartRound();
    }

    private void SaveRoundTime()
    {
        // For adding a new array element to previousRoundTimes
        float [] newTimes = new float[previousRoundTimes.Length + 1];
        previousRoundTimes.CopyTo(newTimes, 0);
        newTimes[newTimes.Length - 1] = currentRoundTime;
        previousRoundTimes = newTimes;
    }
}
