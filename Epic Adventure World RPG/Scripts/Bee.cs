using UnityEngine;

public class Bee : MonoBehaviour
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
            flip_direction(); 
        }
    }

    void SetRandomTarget()
    {
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        targetPosition = defaultPosition + randomDirection * Random.Range(0f, maxDistance);
    }

    void flip_direction()
    {
        if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector2(-1.5f, 1.5f);
        }
        else
        {
            transform.localScale = new Vector2(1.5f, 1.5f); 
        }
    }
}
