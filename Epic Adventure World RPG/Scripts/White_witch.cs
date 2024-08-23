using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class White_witch_enemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 now_position;
    private Vector2 player_position;
    private Vector2 default_Position;

    public int health = 100;
    public int random_attack = 0;

    public float delay_of_idle = 0.2f;
    public float distance_from_player;
    public float distance_from_default;
    public float movement_speed = 3.8f;
    public float delay_of_idle_timer = 0f;

    public bool is_death = false;
    public bool is_running = false;
    public bool attack_state = false;
    public bool go_back_until_reach_default = false;

    void Start()
    {
        transform.localScale = new Vector2(4f, 4f);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        default_Position = transform.position;
        now_position = transform.position;
    }

    void Update()
    {
        if (!is_death && health < 0)
        {
            Die();
        }
        player_position = FindNearestPlayerPosition();
        now_position = transform.position;
        distance_from_default = Vector2.Distance(now_position, default_Position);
        distance_from_player = Vector2.Distance(now_position, player_position);
        delay_of_idle_timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (distance_from_player < 4.5 || attack_state)
        {
            if (distance_from_player < 3)
            {
                Vector2 move_backward = -set_movement_direction(player_position, true, true);
                flip(-move_backward);
                rb.velocity = move_backward * movement_speed;
                is_running = true;
                if (distance_from_player < 1.8)
                {
                    teleport();
                    flip(player_position);
                }
            }
            else if (distance_from_player > 3.5)
            {
                rb.velocity = Vector2.zero;
                is_running = false;
            }
        }
        else if (delay_of_idle_timer > delay_of_idle)
        {
            is_running = true;
            update_movement(player_position);
        }
        change_running_animation();
    }

    // ----------------------------------------------------------------------------------------------------

    Vector2 FindNearestPlayerPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform nearestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlayer = player.transform;
            }
        }
        return nearestPlayer.position;
    }

    // ----------------------------------------------------------------------------------------------------
    //set trigger not set yet
    void Die()
    {
        is_death = true;
        animator.SetTrigger("");
        Invoke("DestroyGameObject", 2.5f);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    // ----------------------------------------------------------------------------------------------------

    Vector2 set_movement_direction(Vector2 targetPosition, bool isNone = false, bool wantMovement = true)
    {
        Vector2 movementDirection = Vector2.zero;

        if (targetPosition != Vector2.zero && !isNone)
        {
            movementDirection = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = movementDirection * movement_speed;
        }
        else if (targetPosition != Vector2.zero && isNone && wantMovement)
        {
            movementDirection = (targetPosition - (Vector2)transform.position).normalized;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        return movementDirection;
    }

    void update_movement(Vector2 player_position)
    {
        if (distance_from_default > 40)
        {
            movement_speed = 3.8f;
            go_back_until_reach_default = true;
        }
        if ((distance_from_default < 40 && distance_from_player < 15) && !go_back_until_reach_default)
        {
            Vector2 movement = set_movement_direction(player_position);
            filp_handle_bug(movement);

        }
        else
        {
            Vector2 direction_to_default = set_movement_direction(default_Position);
            filp_handle_bug(direction_to_default);
            if (distance_from_default < 0.1)
            {
                rb.velocity = Vector2.zero;
                go_back_until_reach_default = false;
            }
        }
    }

    void flip(Vector2 direction)
    {
        if (direction.x < -0.1)
        {
            transform.localScale = new Vector2(-4f, 4f);
        }
        else if (direction.x > 0.1)
        {
            transform.localScale = new Vector2(4f, 4f);
        }
    }

    void filp_handle_bug(Vector2 direction)
    {
        if (is_running)
        {
            flip(direction);
        }
    }

    void change_running_animation()
    {
        if (is_running)
        {
            animator.SetBool("is_running", true);
        }
        else
        {
            animator.SetBool("is_running", false);
        }
    }

    // ----------------------------------------------------------------------------------------------------

    void teleport()
    {
        float xOffset = Mathf.Sign(player_position.x - now_position.x) * 5f;
        Vector2 newPosition = new Vector2(player_position.x + xOffset, player_position.y + 0.2f);
        transform.position = newPosition;
    }


    // ----------------------------------------------------------------------------------------------------


}
