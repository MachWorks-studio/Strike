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

    public bool infiniteRounds = false;
    public int roundsInCircuit = 5;
    public int currentRound = 1;
    [SerializeField] private ERounds currentRoundState;
    [SerializeField] private float currentRoundTime;
    [SerializeField] private float[] previousRoundTimes = { 0f };
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
            ($"Round {currentRound}\n" +
             $"Time Elapsed: {FormatTime(currentRoundTime)}");
    }

    public void StartCircuit()
    {
        currentRound = 1;
        previousRoundTimes = new float []{ 0f };
        StartRound();
    }

    public void EndCircuit()
    {
        if (roundIsRunning)
        {
            EndRound();
            currentRound = 1;
            TargetsManager.Instance.DespawnAllTargets();
        }
    }
    public void SetRoundState(ERounds state)
    {
        currentRoundState = state;
    }

    public void StartRound()
    {
        if (roundIsRunning == false)
        {
            if (currentRound > 1)
            {
                audioSourceGameManager.PlayOneShot(roundStartAudioClip);
            }
            TargetsManager.Instance.SpawnAllTargets();
            SetRoundState(ERounds.Running);
            currentRoundTime = 0f;
            roundIsRunning = true;
            if (currentRound == roundsInCircuit && infiniteRounds == false)
            {
                TargetsManager.Instance.SetContinueToNextRound(false);
            }
            else
            {
                TargetsManager.Instance.SetContinueToNextRound(true);
            }
        }
    }

    public void EndRound()
    {
        if (roundIsRunning)
        {
            SaveRoundTime();
            roundIsRunning = false;
            SetRoundState(ERounds.Completed);
            audioSourceGameManager.PlayOneShot(roundEndAudioClip);
        }
    }
    public void NextRound()
    {
        if (currentRound < roundsInCircuit || infiniteRounds)
        {
            currentRound++;
            StartRound();
        }
        else
        {
            EndRound();
        }
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

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
