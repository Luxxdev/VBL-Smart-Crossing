using UnityEngine;

public class Car : MonoBehaviour
{
    private float carSpeed;
    private float currentCarSpeed;
    private float direction;
    private float despawnX;
    private bool initialized = false;


    public void Initialize(float currentCarSpeed, float carSpeed, float despawnX, float direction)
    {
        this.carSpeed = carSpeed;
        this.currentCarSpeed = currentCarSpeed;
        this.despawnX = despawnX;
        this.direction = direction;
        GameManager.Instance.OnTrafficUpdated.AddListener(ApplyTrafficData);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        transform.Translate(Vector3.right * currentCarSpeed * direction * Time.deltaTime, Space.World);

        if (transform.position.x >= despawnX && direction > 0f || transform.position.x <= despawnX && direction < 0f)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyTrafficData(TrafficStatus status)
    {
        currentCarSpeed = (status.averageSpeed / 100f) * carSpeed;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnTrafficUpdated.RemoveListener(ApplyTrafficData);
    }
}
