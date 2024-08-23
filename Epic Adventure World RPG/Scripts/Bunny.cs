using UnityEngine;

public class Bunny : MonoBehaviour
{
    public float movementSpeed = 2.0f;

    private Rigidbody2D rb;
    private Vector2 defaultPosition;
    private Vector2 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPosition = transform.position;
        targetPosition = GenerateRandomPosition();
    }

    void Update()
    {
        // Move towards the target position
        Vector2 currentPosition = rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        rb.MovePosition(currentPosition + direction * movementSpeed * Time.deltaTime);

        // Check if reached the target position
        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            // Generate a new random target position
            targetPosition = GenerateRandomPosition();
        }

        // Visualize the intended movement direction using Debug.DrawRay
        Debug.DrawRay(currentPosition, direction, Color.red);
    }

    Vector2 GenerateRandomPosition()
    {
        float randomX = Random.Range(-5f, 5f);
        float randomY = Random.Range(-5f, 5f);
        return defaultPosition + new Vector2(randomX, randomY);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("fuck u");
    }
}
