using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject currentPlayerPrefab;
    [SerializeField] private AudioClip roundStartAudioClip;
    [SerializeField] private AudioClip roundEndAudioClip;
    [SerializeField] private AudioClip circuitStartAudioClip;
    [SerializeField] private AudioClip circuitEndAudioClip;
    [SerializeField] private AudioClip countDownAudioClip;
    [SerializeField] private AudioSource audioSourceGameManager;
    [SerializeField] private MenusUI menusUI;
    [SerializeField] private TMP_Text currentRoundTimeText;
	[SerializeField] private TMP_Text allRoundTimesInCircuitText;
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private TMP_Text previousCircuitTimesText;

    public bool infiniteRounds = false;
    public int roundsInCircuit = 5;
    public int currentRound = 1;
    public string currentRoundTimesStringText;
    [SerializeField] private StageData[] stageData;
	private StageData currentStageData;
    public CircuitSavedData CurrentCircuitData => currentStageData.circuitSavedData;
    [SerializeField] private ERounds currentRoundState = ERounds.Completed;
    [SerializeField] private float currentRoundTime;
    [SerializeField] private float[] previousRoundTimes = { 0f };
    public float lastCircuitTime {  get; private set; }
    public float lastCircuitBestTime {  get; private set; }
    private bool roundIsRunning;
    private bool circuitEndedPrematurely = false;
    public bool CircuitEndedPrematurely => circuitEndedPrematurely;
    private Coroutine _countDownCoroutine;
    private Coroutine _finishCircuitCoroutine;
    
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

	void Start()
	{
        ResetCircuitTimesSavedData();
        InitializeStageData(1);
        allRoundTimesInCircuitText.gameObject.SetActive(false);
        currentRoundTimeText.gameObject.SetActive(false);
	}

    private void Update()
    {
        if (!roundIsRunning) return;
        currentRoundTime += Time.deltaTime;

        UpdateRoundTimesText();
    }

    public void SetPlayerPrefab(GameObject playerPrefab)
    {
        currentPlayerPrefab = playerPrefab;
    }
    private void ResetCircuitTimesSavedData()
    {
        foreach (StageData stage in stageData)
        {
            if (stage.circuitSavedData != null)
                stage.circuitSavedData.circuitTimes.Clear();
        }
    }
    private void InitializeStage0()
    {
        menusUI.gameObject.SetActive(false);
        allRoundTimesInCircuitText.gameObject.SetActive(false);
        currentRoundTimeText.gameObject.SetActive(false);
        InitializeStageData(0);
        StartCoroutine(Stage0CountDown());
    }
    private void PlayStage0()
    {
        StartCircuit(false);
    }

    IEnumerator Stage0CountDown()
    {
        yield return new WaitForSeconds(2f);
        PlayStage0();
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
    public void StartCircuit(bool showCountdown)
    {
        if (_countDownCoroutine == null)
        {
            if (showCountdown)
            {
                _countDownCoroutine = StartCoroutine(StartCircuitCountdown());
                countDownText.gameObject.SetActive(true);
            }
            else
            {
                StartRound();
                countDownText.gameObject.SetActive(false);
            }
            currentRoundTimeText.gameObject.SetActive(true);
            currentRound = 1;
            currentRoundTime = 0;
            previousRoundTimes = new float []{ 0f };
            SetRoundState(ERounds.Paused);
            UpdateRoundTimesText();
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
            if (_finishCircuitCoroutine != null)
            {
                StopCoroutine(_finishCircuitCoroutine);
                _finishCircuitCoroutine = null;
            }
            circuitEndedPrematurely = true;
            TargetsManager.Instance.DespawnAllTargets();
            TargetsManager.Instance.SetContinueToNextRound(false);
            TargetsManager.Instance.StopWaitingForNextRound();
            EndRound();
            currentRound = 1;
            SetRoundState(ERounds.Completed);
            countDownText.gameObject.SetActive(false);
            OnEndCircuit?.Invoke();
            circuitEndedPrematurely = false;
        }
    }
    
    private IEnumerator FinishCircuitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        _finishCircuitCoroutine = null;
        SetRoundState(ERounds.Completed);
        OnEndCircuit?.Invoke();
        allRoundTimesInCircuitText.gameObject.SetActive(false);
        currentRoundTimeText.gameObject.SetActive(false);
        audioSourceGameManager.PlayOneShot(circuitEndAudioClip);
        StopCountDown();
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

            allRoundTimesInCircuitText.gameObject.SetActive(true);
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
            if (!circuitEndedPrematurely)
            {
                _finishCircuitCoroutine = StartCoroutine(FinishCircuitAfterDelay(2f));
            }
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
        currentRoundTextStringBuilder.AppendLine($"\n{FormatTime(currentRoundTime)}");
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
            roundTimesString.AppendLine($"{i} - {FormatTime(previousRoundTimes[i])}");
            totalTime += previousRoundTimes[i];
        }
        if (TargetsManager.Instance.willSpawnNextRound == false)
        {
            roundTimesString.AppendLine($"\nTotal - {FormatTime(totalTime)}");
            SaveCircuitTime(totalTime);
        }
        currentRoundTimesStringText = roundTimesString.ToString();
        allRoundTimesInCircuitText.text = roundTimesString.ToString();
    }
    
    private void SaveCircuitTime(float totalTime)
    {
        if (circuitEndedPrematurely)
        {
            Debug.Log("Circuit Ended Prematurely");
            menusUI.SetCircuitTimesText(false);
            return;
        }
        if (currentStageData.circuitSavedData == null)
        {
            Debug.LogWarning($"No matching CircuitSavedData for stage index {currentStageData.name}");
            return;
        }
        
        CircuitSavedData data = currentStageData.circuitSavedData;
        data.AddTime(totalTime);

        lastCircuitTime = totalTime;
        lastCircuitBestTime = data.BestTime();
        
        menusUI.SetCircuitTimesText();

        Debug.Log($"Saved circuit time {FormatTime(totalTime)} for {data.circuitName}. Best: {FormatTime(data.BestTime())}");
    }

    public string FormatTime(float time)
    {
        //int hours = (int)(time / 3600);
        int minutes = (int)(time % 3600 / 60);
        int seconds = (int)(time % 60);
        int centiseconds = (int)(time * 100 % 100);
        
        return $"{minutes:D2}:{seconds:D2}:{centiseconds:D2}";
    }

    public void SwitchHand()
    {
        if (currentPlayerPrefab.TryGetComponent<IPlayerController>(out var playerController))
        {
            playerController.SwitchHand();
        }
        else
        {
            Debug.LogWarning("Current player prefab does not implement IPlayerController");
        }
    }
    public void QuitGame(float delay = 0f)
    {
        if (delay > 0f)
        {
            StartCoroutine(QuitAfterDelay(delay));
        }
        else
        {
            ExecuteQuit();
        }
    }

    private IEnumerator QuitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ExecuteQuit();
    }
    private void ExecuteQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
