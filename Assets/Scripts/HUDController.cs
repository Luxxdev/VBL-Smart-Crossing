using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{

    [SerializeField] private TMP_Text levelLabel;
    [SerializeField] private TMP_Text timerLabel;

    [SerializeField] private TMP_Text densityLabel;
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private TMP_Text weatherLabel;

    [SerializeField] private TMP_Text spawnRateLabel;
    [SerializeField] private TMP_Text carSpeedLabel;
    [SerializeField] private TMP_Text playerSpeedLabel;

    [SerializeField] private TMP_Text endMessageText;
    [SerializeField] private TMP_Text playButtonText;
    [SerializeField] private Button playButton;


    private void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        GameManager.Instance.OnStateChanged.AddListener(OnStateChanged);
        GameManager.Instance.OnTrafficUpdated.AddListener(OnTrafficUpdated);

    }
    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanged.RemoveListener(OnStateChanged);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            UpdateTimer(GameManager.Instance.TimeRemaining);
        }
    }

    private void UpdateTimer(float remaining)
    {
        if (timerLabel == null) return;

        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        timerLabel.text = $"{minutes:00}:{seconds:00}";

    }


    private void OnPlayClicked()
    {
        playButton.gameObject.SetActive(false);
        GameManager.Instance.StartLevel();
    }




    private void OnStateChanged(GameManager.GameState state)
    {
        levelLabel.text = $"Level {GameManager.Instance.currentLevel}";

        switch (state)
        {
            case GameManager.GameState.Loading:
                endMessageText.gameObject?.SetActive(false);
                break;

            case GameManager.GameState.Playing:
                endMessageText.gameObject?.SetActive(false);
                break;

            case GameManager.GameState.LevelComplete:
                endMessageText.gameObject.SetActive(true);
                endMessageText.text = "Nível Concluído!";
                playButtonText.text = "Next Level";
                playButton.gameObject.SetActive(true);

                break;

            case GameManager.GameState.GameOver:
                endMessageText.gameObject.SetActive(true);
                endMessageText.text = "Você perdeu";
                playButtonText.text = "Play Again";
                playButton.gameObject.SetActive(true);
                break;
        }
    }

    private void OnTrafficUpdated(TrafficStatus status)
    {
        if (densityLabel != null)
            densityLabel.text = $"Densidade: {status.vehicleDensity:F2}";

        if (speedLabel != null)
            speedLabel.text = $"Velocidade média: {status.averageSpeed:F1} km/h";

        if (weatherLabel != null)
            weatherLabel.text = $"Clima: {status.weather.ToUpper()}";

        float interval = 1f / Mathf.Max(status.vehicleDensity, 0.01f);
        if (spawnRateLabel != null)
            spawnRateLabel.text = $"Car spawn: {interval:F2}s";

        float unitySpeed = status.averageSpeed / 100f * GameManager.Instance.carSpawner.carSpeed;
        if (carSpeedLabel != null)
            carSpeedLabel.text = $"Vel. Unity: {unitySpeed:F2} u/s";

        float playerSpeed = GameManager.Instance.player.baseSpeed * GameManager.Instance.player.weatherMultiplier;
        if (playerSpeedLabel != null)
            playerSpeedLabel.text = $"Vel. Player: {playerSpeed:F1} u/s";
    }
}
