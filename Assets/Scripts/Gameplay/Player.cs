using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
public class Player : MonoBehaviour
{
    [field: SerializeField] public float baseSpeed { get; private set; } = 5f;
    [SerializeField] private float laneHeight = 1.5f;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxX = 10f;

    private float finishY;
    public float weatherMultiplier { get; private set; } = 1f;
    private bool isActive = false;
    private Vector3 startPosition;


    private void Start()
    {
        startPosition = transform.position;
        finishY = GameManager.Instance.finishLineY;
    }

    private void Update()
    {
        if (!isActive) return;
        HandleInput();
        CheckFinishLine();
    }

    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f)) return;

        float effectiveSpeed = baseSpeed * weatherMultiplier;
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x + h * effectiveSpeed * Time.deltaTime, minX, maxX);
        pos.y = Mathf.Clamp(pos.y + v * effectiveSpeed * Time.deltaTime, minY, finishY);

        transform.position = pos;
    }


    private void CheckFinishLine()
    {
        if (transform.position.y >= finishY)
            GameManager.Instance.NotifyPlayerWon();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            print("colidiu");
            SetActive(false);
            GameManager.Instance.NotifyPlayerDied();
        }
    }

    public void ApplyWeatherMultiplier(string weather)
    {
        weatherMultiplier = weather switch
        {
            "sunny" => 1.0f,
            "clouded" => 0.8f,
            "foggy" => 0.8f,
            "light rain" => 0.6f,
            "heavy rain" => 0.4f,
            _ => 1.0f
        };
    }


    public void SetActive(bool active) => isActive = active;

    public void ResetPosition()
    {
        StopAllCoroutines();
        transform.position = startPosition;
    }
}