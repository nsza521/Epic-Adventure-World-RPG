using UnityEngine;

public class Parry : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private EdgeCollider2D edgeCollider;
    private SpriteRenderer spriteRenderer;

    public bool isRightClick = false;
    public string direction = "right";
    public string prev_direction = "none";

    Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (Input.GetMouseButtonDown(1))
        {
            isRightClick = true;
        }
        update_direction();
    }

    void FixedUpdate()
    {
        if (isRightClick)
        {
            animator.SetBool("is_parry_perfect", true);
            isRightClick = false;
        }
        else
        {
            animator.SetBool("is_parry_perfect", false); 
        }
        if (prev_direction != direction)
        {
            UpdateColliderPoints();
        }
        check_direction();
    }

    void UpdateColliderPoints()
    {
        Vector2[] originalPoints = edgeCollider.points;
        for (int i = 0; i < originalPoints.Length; i++)
        {
            originalPoints[i] = new Vector2(-originalPoints[i].x, originalPoints[i].y);
        }
        edgeCollider.points = originalPoints;
    }

    void check_direction()
    {
        if (direction == "right")
        {
            prev_direction = "right";
            spriteRenderer.flipX = true;
        }
        else if (direction == "left")
        {
            prev_direction = "left";
            spriteRenderer.flipX = false;
        }

        if (movement.x < 0)
        {
            direction = "left";
        }
        else if (movement.x > 0)
        {
            direction = "right";
        }
    }
    void update_direction()
    {
        if (movement.y != 0)
        {
            if (movement.x >= 0)
            {
                direction = "right";
            }
            else
            {
                direction = "left";
            }
        }
    }
}
