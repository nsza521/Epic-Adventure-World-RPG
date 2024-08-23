using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Ghost_Warrior_3 : MonoBehaviour
{

    public Animator animator;
    public Rigidbody2D rb;

    private Vector2 default_Position;
    private Vector2 now_position;

    private Transform nearest_player;

    public float distance_from_player = 0f;
    public float distance_to_player_y = 0f;
    public float distance_from_default = 0f;
    public float movement_speed;
    public float delay_of_idle_timer = 0f;
    public float delay_of_idle = 0.8f;

    private bool attack_state = false;
    private bool is_running = false;
    void Start()
    {
        transform.localScale = new Vector2(transform.localScale.x, 6f);
        rb = GetComponent<Rigidbody2D>();   
        animator = GetComponent<Animator>();
        default_Position = transform.position;
        now_position = transform.position;
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestPlayer = player;
            }
        }
        if (nearestPlayer != null)
        {
            nearest_player = nearestPlayer.transform;
        }
    }

    void Update()
    { 
        FindNearestPlayer();
        now_position = transform.position;
        distance_to_player_y = Mathf.Abs(transform.position.y - nearest_player.position.y);
        distance_from_default = Vector2.Distance(now_position,default_Position);
        distance_from_player = Vector2.Distance(nearest_player.position, now_position);
        delay_of_idle_timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (distance_from_player < 3 && distance_to_player_y <1.5 && delay_of_idle_timer > delay_of_idle)
        {
            Vector2 direction = set_movement_direction(nearest_player.position, true, true);
            flip(direction);
            set_movement_direction(Vector2.zero);
            random_attack_func();
            delay_of_idle_timer = 0f;
        }
        else if (delay_of_idle_timer > delay_of_idle)
        {
            update_movement();
        }
        else
        {
            rb.velocity = Vector2.zero;
            is_running = false;
        }
        update_running();
    }

    Vector2 set_movement_direction(Vector2 targetPosition, bool isNone = false, bool wantMovement = true)
    {
        Vector2 movementDirection = Vector2.zero;

        if (targetPosition != Vector2.zero && !isNone)
        {
            movementDirection = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = movementDirection * movement_speed;
            is_running = true;
        }
        else if (targetPosition != Vector2.zero && isNone && wantMovement)
        {
            movementDirection = (targetPosition - (Vector2)transform.position).normalized;
        }
        else
        {
            rb.velocity = Vector2.zero;
            is_running = false;
        }

        return movementDirection;
    }

    void flip(Vector2 direction)
    {
        if (direction.x < -0.1)
        {
            transform.localScale = new Vector2(-6, 6);
        }
        else if (direction.x > 0.1)
        {
            transform.localScale = new Vector2(6, 6);
        }
    }

    void update_movement()
    {
        Vector2 monster_dircetion;
        if (distance_from_player > 2.7)
        {
            monster_dircetion = set_movement_direction(nearest_player.position);
        }
        else if (distance_to_player_y > 1.5)
        {
            monster_dircetion = set_movement_direction(nearest_player.position, true);
            set_movement_direction(new Vector2(0, nearest_player.position.y));
            is_running = true;
        }
        else
        {
            monster_dircetion = set_movement_direction(Vector2.zero);
        }
        flip(monster_dircetion);
    }

    void update_running()
    {
        if (is_running)
        {
            animator.SetBool("is_moving", true);
        }
        else
        {
            animator.SetBool("is_moving", false);
        }
    }

    // ----------------------------------------------------------------------------------------------------

    public float hitDelay = 0f;
    public int damagePerHit = 0;
    string attack_pattern = "None";
    int random_attack = 0;

    delegate void Attack();

    void random_attack_func()
    {
        Attack[] attacks = { attack_1, attack_2, attack_3 };
        random_attack = Random.Range(0, 3);
        attacks[random_attack]();
    }
    // 1
    void attack_1()
    {
        if (!attack_state && targets != null)
        {
            damagePerHit = Random.Range(3, 6);
            StartCoroutine(AttackRoutine(1));
            attack_pattern_format("attack_1", 0.4f);
        }
    }
    // 2
    void attack_2()
    {
        if (!attack_state && targets != null)
        {
            damagePerHit = Random.Range(5, 11);
            StartCoroutine(AttackRoutine(1));
            attack_pattern_format("attack_2", 0.4f);
        }
    }
    // 3
    void attack_3()
    {
        if (!attack_state && targets != null)
        {
            damagePerHit = Random.Range(8, 11);
            Invoke("delay_of_attack_3", 0.6f);
            attack_pattern_format("attack_3", 0.7f);
        }
    }

    void attack_pattern_format(string attack_pattern_input, float time_duration)
    {
        if (!attack_state)
        {
            attack_state = true;
            attack_pattern = attack_pattern_input;
            animator.SetBool(attack_pattern, true);
            Invoke("AttackPatternToIdle", time_duration);
        }
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

    void delay_of_attack_3()
    {
        StartCoroutine(AttackRoutine(1));
    }

    // ----------------------------------------------------------------------------------------------------

    private bool canAttack = false;
    private List<Collider2D> targets = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targets.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targets.Remove(other);
        }
    }

    void HitTarget(Collider2D target)
    {
        Vector2 monster_direction = set_movement_direction(target.transform.position, true, true);
        target.GetComponent<PlayerHealth>().TakeDamage(damagePerHit, monster_direction);
    }

    IEnumerator AttackRoutine(int hits)
    {
        for (int i = 0; i < hits; i++)
        {
            foreach (var target in targets)
            {
                HitTarget(target);
            }
            yield return new WaitForSeconds(hitDelay);
        }
    }

    public int points;
    void OnDestroy()
    {
        points = Random.Range(39, 79);
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
