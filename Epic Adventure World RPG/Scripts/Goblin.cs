using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Goblin : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public CapsuleCollider2D colliders;

    public int random_attack = 0;
    public int player_irritating_monster_count = 0;

    public float delay_of_idle = 0.2f;
    public float distance_from_player;
    public float distance_from_default;
    public float movement_speed = 3.8f;
    public float delay_of_idle_timer = 0f;

    private Vector2 now_position;
    private Vector2 player_position;
    private Vector2 default_Position;
    private AnimatorStateInfo currentAnimationState;

    public bool is_running = false;
    public bool attack_state = false;
    public bool player_irritating_monster = false;
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
        transform.localScale = new Vector2(transform.localScale.x, 4f);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        default_Position = transform.position;
        now_position = transform.position;
    }

    void Update()
    {
        currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        check_if_player_is_irritating_monster();
        player_position = FindNearestPlayerPosition();
        now_position = transform.position;
        distance_from_default = Vector2.Distance(now_position, default_Position);
        distance_from_player = Vector2.Distance(now_position, player_position);
        delay_of_idle_timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (currentAnimationState.IsName("death"))
        {
            rb.velocity = Vector2.zero;
            Destroy(colliders);
        }
        else
        {
            if (distance_from_player < 2 || attack_state)
            {
                update_attack_state();
                irritataing_monster_count(0);

            }
            else if (delay_of_idle_timer > delay_of_idle)
            {
                update_movement(player_position);
                irritataing_monster_count(1);
            }
        }
        

    }

    void update_attack_state()
    {
        set_back_to_idle(ref is_running);
        if (delay_of_idle_timer > delay_of_idle)
        {
            Vector2 direction = set_movement_direction(player_position, true, true);
            flip_goblin(direction);
            random_attack_func();
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

    public float hitDelay = 0f;
    public int damagePerHit = 0;
    string attack_pattern = "None";

    delegate void Attack();

    void random_attack_func()
    {
        Attack[] attacks = { light_attack, attack_combo, dash_attack };
        random_attack = Random.Range(0, 3);
        attacks[random_attack]();
    }
    // 1
    void light_attack()
    {
        if (!attack_state && target != null)
        {   
            damagePerHit = Random.Range(3, 6);
            StartCoroutine(AttackRoutine(target, 2));
            attack_pattern_format("is_light_attack", 20f, 0.85f);
        }
    }
    // 2
    void attack_combo()
    {
        if (!attack_state && target != null)
        {
            damagePerHit = Random.Range(5, 11);
            StartCoroutine(AttackRoutine(target, 3));
            attack_pattern_format("is_combo_attack", 20f, 1.55f);
        }
    }
    // 3
    void dash_attack()
    {
        if (!attack_state && target != null)
        {
            damagePerHit = Random.Range(8, 11);
            StartCoroutine(AttackRoutine(target, 1));
            attack_pattern_format("is_dash_attack", 35f, 0.85f);
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
            AdjustPosition(0.0f, 0.3f);
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
            AdjustPosition(0.0f, -0.3f);
            attack_state = false;
            animator.SetBool(attack_pattern, false);
            delay_of_idle_timer = 0f;
            attack_pattern = "";
        }
    }

    void AdjustPosition(float xOffset, float yOffset)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.x += xOffset;
        newPosition.y += yOffset;
        transform.localPosition = newPosition;
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
        if (distance_from_default > 10000)
        {
            movement_speed = 3.8f;
            go_back_until_reach_default = true;
        }
        if ((distance_from_default < 10000 && distance_from_player < 10000) && !go_back_until_reach_default)
        {
            Vector2 goblin_movement = set_movement_direction(player_position);
            filp_goblin_handle_bug(goblin_movement);

        }
        else
        {
            Vector2 direction_to_default = set_movement_direction(default_Position);
            filp_goblin_handle_bug(direction_to_default);
            if (distance_from_default < 0.1)
            {
                go_back_until_reach_default = false;
                rb.velocity = Vector2.zero;
            }
        }
        change_running_animation();
    }

    void flip_goblin(Vector2 direction)
    {
        if (direction.x < -0.1)
        {
            transform.localScale = new Vector2(-4f, 4f);
        }
        else if (direction.x > 0.1)
        {
            transform.localScale = new Vector2(4f,4f);
        }
    }

    void filp_goblin_handle_bug(Vector2 direction)
    {
        if (is_running)
        {
            flip_goblin(direction);
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

    void check_if_player_is_irritating_monster()
    {
        if (player_irritating_monster_count > 25)
        {
            player_irritating_monster = true;
        }

        if (player_irritating_monster)
        {
            movement_speed = 4.4f;
            delay_of_idle = 0f;
        }
        else
        {
            movement_speed = 3.8f;
            delay_of_idle = 0.2f;
        }
    }

    void irritataing_monster_count(int even_or_odd)
    {
        if (player_irritating_monster_count % 2 == even_or_odd)
        {
            player_irritating_monster_count += 1;
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
        Vector2 goblin_direction = set_movement_direction(player_position, true, true);
        if (canAttack)
        {
            target.GetComponent<PlayerHealth>().TakeDamage(damagePerHit , goblin_direction);
        }
    }

    IEnumerator AttackRoutine(Collider2D target, int hits)
    {
        for (int i = 0; i < hits; i++)  
        {
            HitTarget(target);
            yield return new WaitForSeconds(hitDelay);
        }
    }

    public int points;
    void OnDestroy()
    {
        points = Random.Range(19, 39);
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
