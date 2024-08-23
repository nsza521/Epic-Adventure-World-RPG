using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class Slime : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public CapsuleCollider2D colliders;

    public int health = 100;

    public float delay_of_idle = 0.2f;
    public float distance_from_player;
    public float distance_from_default;
    public float movement_speed = 2f;
    public float delay_of_idle_timer = 0f;

    private Vector2 now_position;
    private Vector2 player_position;
    private Vector2 default_Position;
    private AnimatorStateInfo currentAnimationState;



    public bool is_death = false;
    public bool is_running = false;
    public bool attack_state = false;
    public bool go_back_until_reach_default = false;

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

    void Start()
    {
        transform.localScale = new Vector2(transform.localScale.x, 3.46f);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        default_Position = transform.position;
        now_position = transform.position;
        colliders = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        player_position = FindNearestPlayerPosition();
        now_position = transform.position;
        distance_from_default = Vector2.Distance(now_position, default_Position);
        distance_from_player = Vector2.Distance(now_position, player_position);
        delay_of_idle_timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (currentAnimationState.IsName("water_slime_die") || currentAnimationState.IsName("fire_die") || currentAnimationState.IsName("die") || currentAnimationState.IsName("leaf_slime_die"))
        {
            rb.velocity = Vector2.zero;
            Destroy(colliders);
        }
        else
        {
            if (distance_from_player < 1.2 || attack_state)
            {
                update_attack_state();
            }
            else if (delay_of_idle_timer > delay_of_idle)
            {
                update_movement(player_position);
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------

    void Die()
    {
        is_death = true;
        animator.SetTrigger("is_death");
        Invoke("DestroyGameObject", 2.5f);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    // ----------------------------------------------------------------------------------------------------

    void update_attack_state()
    {
        set_back_to_idle(ref is_running);
        if (delay_of_idle_timer > delay_of_idle)
        {
            Vector2 direction = set_movement_direction(player_position, true, true);
            flip_enemy(direction);
            attack_func();
        }
    }

    void set_back_to_idle(ref bool variable)
    {
        set_movement_direction(Vector2.zero, true);
        if (variable)
        {
            variable = false;
            animator.SetBool("is_running", false);
            delay_of_idle_timer = 0f;
        }
    }

    // ----------------------------------------------------------------------------------------------------

    void hurt()
    {

    }


    // ----------------------------------------------------------------------------------------------------


    public float hitDelay = 0.25f;
    public int damagePerHit = 0;
    string attack_pattern = "None";

    delegate void Attack();

    void attack_func()
    {
        if (!attack_state && target != null)
        {
            damagePerHit = Random.Range(1, 6);
            StartCoroutine(AttackRoutine(target, 1));
            attack_pattern_format("is_attack", 20f, 0.85f);
        }
    }
    
    void attack_pattern_format(string attack_pattern_input, float dash_speed, float time_duration)
    {
        Vector2 player_position_now = player_position;
        if (!attack_state)
        {
            attack_state = true;
            dash_movement(player_position_now, dash_speed);
            attack_pattern = attack_pattern_input;
            animator.SetBool(attack_pattern, true);
            Invoke("AttackPatternToIdle", time_duration);
        }
    }

    void dash_movement(Vector2 position_to_go, float speed)
    {
        Vector2 movement_direction = (position_to_go - (Vector2)transform.position).normalized;
        rb.velocity = movement_direction * speed;
    }

    void AttackPatternToIdle()
    {
        if (attack_state && attack_pattern != "")
        {
            attack_state = false;
            animator.SetBool(attack_pattern, false);
            delay_of_idle_timer = 0f;
            attack_pattern = "";
        }
    }

    // ----------------------------------------------------------------------------------------------------

    // Check if input position is not (0,0) and movement is desired         if
    // Check if input position is not (0,0) and only direction is needed    else if 
    // No movement required                                                 else
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
        // Skip
        if (distance_from_default > 10000)
        {
            movement_speed = 2f;
            go_back_until_reach_default = true;
        }
        //Skip
        if ((distance_from_default < 10000 && distance_from_player < 10000) && !go_back_until_reach_default)
        {
            Vector2 enemy_movement = set_movement_direction(player_position);
            filp_enemy_handle_bug(enemy_movement);

        }
        else
        {
            Vector2 direction_to_default = set_movement_direction(default_Position);
            filp_enemy_handle_bug(direction_to_default);
            if (distance_from_default < 0.1)
            {
                go_back_until_reach_default = false;
                rb.velocity = Vector2.zero;
            }
        }
        change_running_animation();
    }

    void flip_enemy(Vector2 direction)
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

    void filp_enemy_handle_bug(Vector2 direction)
    {
        if (is_running)
        {
            flip_enemy(direction);
        }
    }

    void change_running_animation()
    {
        if (distance_from_default > 0.1)
        {
            is_running = true;
            animator.SetBool("is_running", true);
        }
        else
        {
            is_running = false;
            animator.SetBool("is_running", false);
        }
    }

    // ----------------------------------------------------------------------------------------------------

    private bool canAttack = true;
    Collider2D target;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = true;
            target = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = false;
            target = null;
        }
    }

    void HitTarget(Collider2D target)
    {
        Vector2 enemy_direction = set_movement_direction(player_position, true, true);
        if (canAttack)
        {
            target.GetComponent<PlayerHealth>().TakeDamage(damagePerHit, enemy_direction);
        }
    }

    IEnumerator AttackRoutine(Collider2D target, int hits)
    {
        for (int i = 0; i < hits; i++)
        {
            yield return new WaitForSeconds(hitDelay);
        }

        yield return new WaitForSeconds(0.25f); // Wait for 0.25 seconds before hitting
        HitTarget(target);
    }

    public int points ; 
    void OnDestroy()
    {
        points = Random.Range(10, 19);
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

        // Check if the ScoreManager instance is found
        if (scoreManager != null)
        {
            ScoreManager.AddScore(points); // Add points to the score
        }
        else
        {
            Debug.Log("ScoreManager instance not found when trying to add score!");
        }
    }


}
