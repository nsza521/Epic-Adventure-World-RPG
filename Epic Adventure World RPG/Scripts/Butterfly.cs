using UnityEngine;

public class Butterfly : MonoBehaviour
{
    public float moveSpeed = 0.3f;
    public float maxDistance = 2f;

    private Vector2 defaultPosition; 
    private Vector2 targetPosition;

    void Start()
    {
        defaultPosition = transform.position;
        SetRandomTarget();
        moveSpeed = 0.3f;
        maxDistance = 2f;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomTarget();
        }
    }

    void SetRandomTarget()
    {
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        targetPosition = defaultPosition + randomDirection * Random.Range(0f, maxDistance);
    }
}
