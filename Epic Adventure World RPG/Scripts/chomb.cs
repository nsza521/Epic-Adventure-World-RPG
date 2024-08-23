using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class chomb : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public AudioClip[] musicTracks;
    public AudioSource audioSource;

    public float movement_speed = 2f;
    public float maxDistance = 4f;
    public bool gethit = false;
    public bool is_moving = false;

    public Transform target;  
    private Vector2 default_Position;
    private Vector2 position_to_move;
    private Transform nearest_player;

    public float delay_of_idle_timer = 0f;

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
    public Monster_health chomb_health;
    private int defaulthealth;
    void Start()
    {
        chomb_health = GetComponent<Monster_health>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        default_Position = transform.position;
        SetRandomTarget();
        defaulthealth = chomb_health.health;
        audioSource.volume = 0.5f;
    }
    bool playone = true;
    private float timer = 0f;
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 0.5f && playone && gethit)
        {
            int randomIndex = Random.Range(0, musicTracks.Length);
            audioSource.PlayOneShot(musicTracks[randomIndex]);
            playone = false;
            timer = 0f;
        }
        else
        {
            playone = true;
        }
        if (defaulthealth > chomb_health.health)
        {
            gethit = true;
            defaulthealth = chomb_health.health;
        }
        if(gethit)
        {
            
            Invoke("hurt_animation",0.2f);
        }
        else
        {
            FindNearestPlayer();
            movearound();
            delay_of_idle_timer += Time.deltaTime;
            if (is_moving)
            {
                animator.SetBool("is_moving", true);
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("is_moving", false);
            }
        }
    }
    void hurt_animation()
    {
        movement_speed = 3f;
        FindNearestPlayer();
        
        animator.SetBool("hurt", true);
        animator.SetBool("is_moving", false);
        rb.velocity = Vector2.zero;
        Invoke("set_back", 0.3f);
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

    void flip(Vector2 direction)
    {
        if (direction.x < -0.1)
        {
            transform.localScale = new Vector2(4, 4);
        }
        else if (direction.x > 0.1)
        {
            transform.localScale = new Vector2(-4, 4);
        }
    }

    void SetRandomTarget()
    {
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Vector2 target_position = default_Position + randomDirection * Random.Range(0f, maxDistance);
        position_to_move = target_position;
    }

    float delay_idle = 0f;

    void movearound()
    {
        if (Vector2.Distance(transform.position, position_to_move) < 0.1f)
        {
            rb.velocity = Vector2.zero;
            is_moving = false ;
            SetRandomTarget();
            delay_idle = Random.Range(3f, 10f);
            delay_of_idle_timer = 0f;
        }
        if (delay_of_idle_timer > delay_idle)
        {
            is_moving = true;
            Vector2 direction = set_movement_direction(position_to_move);
            flip(direction);
        }
        else
        {
            is_moving = false;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    void set_back()
    {
        animator.SetBool("hurt", false);
        gethit = false;
    }
    // ----------------------------------------------------------------------------------------------------

    public float hitDelay = 0f;
    private bool canAttack = false;
    public int damagePerHit = 0;
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
        points = Random.Range(9, 15);
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
