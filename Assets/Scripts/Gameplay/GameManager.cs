
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public CarSpawner carSpawner { get; private set; }
    [field: SerializeField] public Player player { get; private set; }

    [SerializeField] private TrafficApiService apiService;
    [SerializeField] private HUDController HUD;

    public enum GameState { Idle, Loading, Playing, LevelComplete, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Idle;
    public int currentLevel = 1;
    public float finishLineY = 40f;
    public float TimeRemaining { get; private set; } = 0f;

    public UnityEvent<GameState> OnStateChanged = new();
    public UnityEvent<TrafficStatus> OnTrafficUpdated = new();


    private void Awake()
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
        if (CurrentState == GameState.Playing)
        {
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0f)
            {
                TimeRemaining = 0f;
                NotifyPlayerDied();
            }
        }
    }

    public void StartLevel()
    {
        player.SetActive(true);
        player.ResetPosition();
        SetState(GameState.Loading);
        StopAllCoroutines();

        apiService.FetchTrafficStatus(OnApiSuccess, OnApiError);
    }

    private void OnApiSuccess(TrafficResponse response)
    {
        float totalTime = CalculateTotalTime(response.predicted_status);
        TimeRemaining = totalTime;

        ApplyTrafficStatus(response.current_status);

        StartCoroutine(SchedulePredictions(response.predicted_status));

        player.SetActive(true);
        player.ResetPosition();

        SetState(GameState.Playing);
    }

    private void OnApiError(string error)
    {
        Debug.LogError(error);
        SetState(GameState.Idle);
    }


    private IEnumerator SchedulePredictions(List<PredictedEntry> predictions)
    {
        if (predictions == null) yield break;

        foreach (PredictedEntry entry in predictions)
        {
            float delaySeconds = entry.estimated_time / 1000f;
            yield return new WaitForSeconds(delaySeconds);

            if (CurrentState != GameState.Playing) yield break;

            ApplyTrafficStatus(entry.predictions);
        }
    }

    private float CalculateTotalTime(List<PredictedEntry> predictions)
    {
        if (predictions == null || predictions.Count == 0)
            return 30f; // fallback caso a API não retorne predições

        return predictions[predictions.Count - 1].estimated_time / 1000f;
    }


    public void NotifyPlayerWon()
    {
        SetState(GameState.LevelComplete);
        // StopAllCoroutines();
        // carSpawner.StopSpawning();
        player.SetActive(false);

        currentLevel++;

    }
    public void NotifyPlayerDied()
    {
        SetState(GameState.GameOver);
        // carSpawner.StopSpawning();
        player.SetActive(false);
        currentLevel = 1;

    }

    private void ApplyTrafficStatus(TrafficStatus status)
    {
        carSpawner.ApplyTrafficData(status);
        player.ApplyWeatherMultiplier(status.weather);
        OnTrafficUpdated.Invoke(status);
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged.Invoke(newState);
    }


}
