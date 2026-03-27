using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float offset = 2f;
    [SerializeField] private float xMax = 1f;
    [SerializeField] private float xMin = -1f;
    [SerializeField] private float yMax = 1f;
    [SerializeField] private float yMin = 0f;


    private void LateUpdate()
    {
        if (target == null) return;

        float targetX = transform.position.x;
        float targetY = transform.position.y;

        if (Mathf.Abs(transform.position.x - target.position.x) > offset)
            targetX = target.position.x;

        if (Mathf.Abs(transform.position.y - target.position.y) > offset)
            targetY = target.position.y;

        targetX = Mathf.Clamp(targetX, xMin, xMax);
        targetY = Mathf.Clamp(targetY, yMin, yMax);

        transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, targetY, transform.position.z), 5 * Time.deltaTime);
        // transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}