using UnityEngine;

public class Bird : MonoBehaviour
{
    public float moveSpeed;
    public float maxDistance;
    public float delay_of_idle_timer;
    public float delay_of_idle = 3f;

    public Animator animator;
    public Rigidbody2D rb;

    private Vector2 defaultPosition;
    private Vector2 targetPosition;

    //font back left right
    private string current_idle = "None";
    private string behavior = "None";
    private bool doublePeckTriggered = false;
    void Start()
    {
        defaultPosition = transform.position;
        moveSpeed = 0.8f;
        maxDistance = 2.5f;
        delay_of_idle = 3f;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (current_idle == "None" && behavior=="None")
        {
            SetRandomDirection();
            Set_random_behavior();
        }
        delay_of_idle_timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (behavior != "None" && current_idle != "None" && timer())
        {
            behavior_do(behavior,current_idle);
        }
    }
    
    bool timer()
    {
        if(delay_of_idle_timer > delay_of_idle)
        {
            return true;
        }
        return false;
    }

    void to_idle()
    {
        animator.SetTrigger("to_idle");
        delay_of_idle_timer = 0f;
    }

    void double_peck()
    {
        doublePeckTriggered = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("double_peck");
        Invoke("to_idle", 1f);
    }

    void move(string direction)
    {
        Vector2 move_diection = Vector2.zero;
        switch (direction)
        {
            case "Up": move_diection = new Vector2(0, 1); break;
            case "Down": move_diection = new Vector2(0, -1); break;
            case "Left": move_diection = new Vector2(-1, 0); break;
            case "Right": move_diection = new Vector2(1, 0); break;
        }
        if (!doublePeckTriggered)
        {
            rb.velocity = move_diection * 0.8f;
        }
    }

    void SetRandomDirection()
    {
        switch (Random.Range(0, 4))
        {
            case 0: current_idle = "Up"; break;
            case 1: current_idle = "Down"; break;
            case 2: current_idle = "Left"; break;
            case 3: current_idle = "Right"; break;
        }
        current_idle = "Down"; 
    }

    void Set_random_behavior()
    {
        switch (Random.Range(0, 4))
        {
            case 0: behavior = "sing"; break;
            case 1: behavior = "walk"; break;
            case 2: behavior = "walk"; break;
            case 3: behavior = "sing"; break;
        }
    }

    void behavior_do(string behavior_input, string direction)
    {

        switch (behavior_input)
        {
            case "sing":   animator.SetTrigger(behavior_input); Invoke("to_idle", 1.5f); break;
            case "walk": animator.SetTrigger(behavior_input); move(direction); Invoke("double_peck",1.5f);  ; break;
            case "sit_sleep": behavior = "walk"; break;
        }
        behavior = "None";
        current_idle = "None";
        doublePeckTriggered = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("is_player_nearby");
        }
    }
    
   
}
