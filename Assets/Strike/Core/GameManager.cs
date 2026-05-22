using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private AudioClip roundStartAudioClip;
    [SerializeField] private AudioClip roundEndAudioClip;
    private AudioSource audioSourceGameManager;
    [SerializeField] private TMP_Text gameManagerText;
    
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
        
        audioSourceGameManager = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if (!roundIsRunning) return;
        currentRoundTime += Time.deltaTime;
        
        UpdateGameManagerText
            ($"Round {roundsElapsed}\n" +
             $"Time Elapsed: {FormatTime(currentRoundTime)}");
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
        if (roundsElapsed > 0)
        {
            audioSourceGameManager.PlayOneShot(roundStartAudioClip);
        }
    }

    public void CompleteRound()
    {
        roundIsRunning = false;
        SetRoundState(ERounds.Completed);
        SaveRoundTime();
        audioSourceGameManager.PlayOneShot(roundEndAudioClip);
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

    private void UpdateGameManagerText(string text)
    {
        gameManagerText.text = text;
    }

    string FormatTime(float time)
    {
        int hours = (int)(time / 3600);
        int minutes = (int)(time % 3600 / 60);
        int seconds = (int)(time % 60);
        int centiseconds = (int)(time * 100 % 100);
        
        return $"{hours}:{minutes}:{seconds}:{centiseconds}";
    }
}
