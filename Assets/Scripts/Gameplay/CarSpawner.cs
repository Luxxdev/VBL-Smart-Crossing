
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{

    [field: SerializeField] public float carSpeed { get; private set; } = 15f;
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private float laneHeight = 5f;
    [SerializeField] private int totalLanes = 5;

    private float maxLaneY;
    private List<float> laneYPositions = new();
    private float currentSpawnInterval = 1f;
    private float currentCarSpeed = 7.5f;
    private bool isSpawning = false;
    private float lastPlayerPosY = 0;
    private Coroutine spawnCoroutine;


    private void Start()
    {
        lastPlayerPosY = GameManager.Instance.player.transform.position.y;
        maxLaneY = GameManager.Instance.finishLineY;
        StartSpawning();
    }

    private void SetLanes()
    {
        float playerY = GameManager.Instance.player.transform.position.y;
        if (Mathf.Abs(playerY - lastPlayerPosY) < laneHeight && laneYPositions.Count != 0) return;

        lastPlayerPosY = playerY;

        float snappedY = Mathf.Ceil(playerY / laneHeight) * laneHeight + laneHeight;
        if (snappedY + laneHeight * (totalLanes - 1) >= maxLaneY) return;

        laneYPositions.Clear();

        for (int i = 0; i < totalLanes; i++)
        {
            laneYPositions.Add(snappedY + laneHeight * i);
        }
    }

    public void ApplyTrafficData(TrafficStatus status)
    {
        float density = Mathf.Clamp(status.vehicleDensity, 0.1f, 1.0f);
        currentSpawnInterval = 1f / density;
        currentCarSpeed = (status.averageSpeed / 100f) * carSpeed;

        if (isSpawning)
        {
            StopSpawning();
            StartSpawning();
        }
    }

    private void StartSpawning()
    {
        if (isSpawning) return;
        SetLanes();
        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    private void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            SpawnCar();
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private void SpawnCar()
    {
        if (carPrefab == null || leftSpawnPoint == null) return;
        int lane = Random.Range(0, laneYPositions.Count - 1);
        float direction = lane % 2 == 0 ? 1f : -1f;

        Transform spawnPoint = direction > 0f ? leftSpawnPoint : rightSpawnPoint;
        Transform despawnPoint = direction > 0f ? rightSpawnPoint : leftSpawnPoint;

        Vector3 spawnPos = new(spawnPoint.position.x, laneYPositions[lane], spawnPoint.position.z);

        GameObject car = Instantiate(carPrefab, spawnPos, Quaternion.identity);

        Car carScript = car.GetComponent<Car>() ?? car.AddComponent<Car>();

        carScript.Initialize(currentCarSpeed, carSpeed, despawnPoint.position.x, direction);
        SetLanes();
    }

    private void OnDestroy() => StopSpawning();
}
