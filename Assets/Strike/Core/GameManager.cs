using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private AudioClip roundStartAudioClip;
    [SerializeField] private AudioClip roundEndAudioClip;
    [SerializeField] private AudioClip circuitStartAudioClip;
    [SerializeField] private AudioClip circuitEndAudioClip;
    [SerializeField] private AudioClip countDownAudioClip;
    private AudioSource audioSourceGameManager;
    [SerializeField] private TMP_Text currentRoundTimeText;
	[SerializeField] private TMP_Text allRoundTimesInCircuitText;
    [SerializeField] private TMP_Text countDownText;

    public bool infiniteRounds = false;
    public int roundsInCircuit = 5;
    public int currentRound = 1;
    [SerializeField] private StageData[] stageData;
	private StageData currentStageData;
    [SerializeField] private ERounds currentRoundState = ERounds.Completed;
    [SerializeField] private float currentRoundTime;
    [SerializeField] private float[] previousRoundTimes = { 0f };
    
    private bool roundIsRunning;
    private Coroutine _countDownCoroutine;
    
    public static event System.Action OnStartCircuit;
    public static event System.Action OnEndCircuit;
    public static event System.Action OnStartRound;
    public static event System.Action OnEndRound;
    public static event System.Action OnNextRound;
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

    private void Update()
    {
        if (!roundIsRunning) return;
        currentRoundTime += Time.deltaTime;

        UpdateRoundTimesText();
    }

    public void InitializeStageData(int stageNumber)
    {
        currentStageData = stageData[stageNumber];
        roundsInCircuit = currentStageData.roundTargetData.Length - 1;
        infiniteRounds = currentStageData.infiniteRounds;
        TargetsManager.Instance.InitializeRoundTargetData(currentStageData.roundTargetData);
        if (infiniteRounds)
            Debug.Log("Infinite rounds in circuit");
        else
            Debug.Log($"{currentStageData.roundTargetData.Length} Rounds in circuit");
    }
    public void StartCircuit()
    {
        if (_countDownCoroutine == null)
        {
            _countDownCoroutine = StartCoroutine(StartCircuitCountdown());
            currentRound = 1;
            previousRoundTimes = new float []{ 0f };
            SetRoundState(ERounds.Paused);
            UpdateRoundTimesText();
            countDownText.gameObject.SetActive(true);
            OnStartCircuit?.Invoke();
        }
    }

    IEnumerator StartCircuitCountdown()
    {
        int counter = 3;
        while (counter > 0)
        {
            countDownText.text = counter.ToString();
            audioSourceGameManager.PlayOneShot(countDownAudioClip);
            yield return new WaitForSeconds(1f);
            counter--;
        }
        countDownText.gameObject.SetActive(false);
        StartRound();
    }

    public void EndCircuit()
    {
        if (currentRoundState == ERounds.Paused || currentRoundState == ERounds.Running)
        {
            if (_countDownCoroutine != null)
            {
                StopCoroutine(_countDownCoroutine);
                _countDownCoroutine = null;
            }
            TargetsManager.Instance.DespawnAllTargets();
            TargetsManager.Instance.SetContinueToNextRound(false);
            TargetsManager.Instance.StopWaitingForNextRound();
            EndRound();
            currentRound = 1;
            SetRoundState(ERounds.Completed);
            audioSourceGameManager.PlayOneShot(circuitEndAudioClip);
            UpdateCurrentCircuitTimesText();
            countDownText.gameObject.SetActive(false);
            OnEndCircuit?.Invoke();
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
            if (currentRound == 1)
            {
                audioSourceGameManager.PlayOneShot(circuitStartAudioClip);
            }
            else if (currentRound > 1)
            {
                audioSourceGameManager.PlayOneShot(roundStartAudioClip);
            }

            int roundDataIndex = infiniteRounds ? 0 : currentRound;
            TargetsManager.Instance.UpdateTargetCount(roundDataIndex);
            TargetsManager.Instance.SpawnAllTargets(roundDataIndex);
            CoverManager.Instance.SetCoverMesh(currentStageData.roundTargetData[roundDataIndex]);
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
            UpdateCurrentCircuitTimesText();
        }
        OnStartRound?.Invoke();
    }

    public void EndRound()
    {
        roundIsRunning = false;
        SaveRoundTime();
        UpdateCurrentCircuitTimesText();
        if (TargetsManager.Instance.willSpawnNextRound == false)
        {
            SetRoundState(ERounds.Completed);
            OnEndCircuit();
            audioSourceGameManager.PlayOneShot(circuitEndAudioClip);
            StopCountDown();
        }
        else
        {
            SetRoundState(ERounds.Paused);
            audioSourceGameManager.PlayOneShot(roundEndAudioClip);
        }
        OnEndRound?.Invoke();
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
        OnNextRound?.Invoke();
    }

    private void StopCountDown()
    {
        if (_countDownCoroutine != null)
        {
            StopCoroutine(_countDownCoroutine);
            _countDownCoroutine = null;
        }
    }

    private void SaveRoundTime()
    {
        // For adding a new array element to previousRoundTimes
        float [] newTimes = new float[previousRoundTimes.Length + 1];
        previousRoundTimes.CopyTo(newTimes, 0);
        if (TargetsManager.Instance.targetsRemaining == 0)
        {
            newTimes[newTimes.Length - 1] = currentRoundTime;
        }
        else
        {
            newTimes[newTimes.Length - 1] = 0f;
        }
        previousRoundTimes = newTimes;
    }

    private void UpdateRoundTimesText()
    {
        System.Text.StringBuilder currentRoundTextStringBuilder = new System.Text.StringBuilder();
        currentRoundTextStringBuilder.Append($"Round {currentRound}");
        if (infiniteRounds == false)
        {
            currentRoundTextStringBuilder.Append($"/{roundsInCircuit}");
        }
        currentRoundTextStringBuilder.AppendLine($"\nTime: {FormatTime(currentRoundTime)}");
        currentRoundTimeText.text = currentRoundTextStringBuilder.ToString();
    }

    private void UpdateCurrentCircuitTimesText()
    {
        System.Text.StringBuilder roundTimesString = new System.Text.StringBuilder();
        roundTimesString.AppendLine("Round Times\n");
        float totalTime = 0f;

        for (int i = 1; i < previousRoundTimes.Length; i++)
        {
            if (previousRoundTimes[i] == 0) break;
            roundTimesString.AppendLine($"{i}: {FormatTime(previousRoundTimes[i])}");
            totalTime += previousRoundTimes[i];
        }
        if (TargetsManager.Instance.willSpawnNextRound == false)
        {
            roundTimesString.AppendLine($"\nTotal: {FormatTime(totalTime)}");
        }
        allRoundTimesInCircuitText.text = roundTimesString.ToString();
    }

    string FormatTime(float time)
    {
        int hours = (int)(time / 3600);
        int minutes = (int)(time % 3600 / 60);
        int seconds = (int)(time % 60);
        int centiseconds = (int)(time * 100 % 100);
        
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}:{centiseconds:D2}";
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
