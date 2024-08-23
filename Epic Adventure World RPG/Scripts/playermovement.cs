using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4.5f;
    public float normalSpeed = 4.5f;
    public float stairSpeed = 3.8f;
    public float delayBetweenPlays = 1.5f;
    private float dashSpeed = 40f;  // Dash speed
    public float dashDuration = 0.2f; // Dash duration
    private int damage = 10;

    private bool isRightClick = false;
    private bool isRightclickhold = false;
    private bool isLeftClick = false;
    private bool is_rolling = false;
    private bool is_ultimate = false;
    private bool is_dashing = false; // Dash state
    private bool is_buff = false; // buff state
    private bool is_healing = false; // heal state


    bool enable_move = true;
    bool isOnStair = false;

    private float lastPlayTime;
    private float dashEndTime; // Time when dash ends

    private Vector2 movement;
    private AnimatorStateInfo currentAnimationState;
    private Transform playerTransform;

    public Rigidbody2D rb;
    public Animator animator;
    public AudioClip fire_sword;
    public SpriteRenderer spriteRenderer;

    private float cooldown_skill_e = 5f;
    private float cooldown_skill_q = 10f;
    private float cooldown_skill_c = 8f;
    private float cooldown_skill_shift = 0.6f;

    public float timer_e;
    public float timer_q;
    public float timer_c;
    public float timer_shift;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        playerTransform = rb.GetComponent<Transform>();
        lastPlayTime = -delayBetweenPlays;
    }

    void Update()
    {
        enable_move = PlayerHealth.instance.enable_move;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);

        if (Input.GetMouseButtonDown(1))
        {
            isRightClick = true;
        }
        if (Input.GetMouseButton(1))
        {
            isRightclickhold = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays)
            {
                isLeftClick = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays && !is_dashing && Time.time >= timer_shift)
            {
                is_rolling = true;
                timer_shift = Time.time + cooldown_skill_shift;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays && Time.time >= timer_e)
            {
                is_ultimate = true;
                timer_e = Time.time + cooldown_skill_e;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays && Time.time >= timer_q)
            {
                damage = 30;
                is_buff = true;
                timer_q = Time.time + cooldown_skill_q;

            }
        }
   
        if (Input.GetKeyDown(KeyCode.C) && Time.time >= timer_c)
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays)
            {
                is_healing = true;
                timer_c = Time.time + cooldown_skill_c;
            }
        }
        moveSpeed = isOnStair ? stairSpeed : normalSpeed;
    }

    void FixedUpdate()
    {
        if(Time.time >= timer_q)
        {
            damage = 10;
        }
        if (is_dashing)
        {
            HandleDash();
            return; 
        }

        if (currentAnimationState.IsName("normal_atk") || currentAnimationState.IsName("parry") || currentAnimationState.IsName("parrying"))
        {
            moveSpeed = 2.5f;
        }
        else if (currentAnimationState.IsName("roll"))
        {
            moveSpeed = 6f;
        }
        else if (currentAnimationState.IsName("ultimate") || currentAnimationState.IsName("ultimate_2") || currentAnimationState.IsName("ultimate_4") || (currentAnimationState.IsName("die_hold")))
        {
            moveSpeed = 0f;
        }
        else
        {
            moveSpeed = normalSpeed;
        }

        if (movement.magnitude != 0 && enable_move && !(currentAnimationState.IsName("die") || currentAnimationState.IsName("die_loop")))
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.deltaTime);
            animator.SetBool("is_moving", true);

            if (movement.x > 0)
            {
                playerTransform.localScale = new Vector3(4, 4,1); 
            }
            else if (movement.x < 0)
            {
                playerTransform.localScale = new Vector3(-4, 4,1); 
            }
        }
        else
        {
            animator.SetBool("is_moving", false);
        }
        handle_knockback();
        parry_animation();
        is_parry_still();
        normal_attack();
        rolling();
        ultimate_1();
        buff();
        healing();
    }
    public GameObject targetObject;
    private PlayerHealth playerhealth;  
    void handle_knockback()
    {
        playerhealth = targetObject.GetComponent<PlayerHealth>();
        if(!playerhealth.takedamage)
        {
            rb.velocity = Vector2.zero;
        }

    }
void StartDash()
    {
        is_dashing = true;
        dashEndTime = Time.time + dashDuration;

        if (targets.Count > 0)
        {
            foreach (var target in targets)
            {
                StartCoroutine(AttackRoutine(target, 2));
            }
        }
    }

    void HandleDash()
    {
        Vector2 dashDirection = new Vector2(Mathf.Sign(playerTransform.localScale.x), 0); // Dash direction based on facing

        rb.velocity = dashDirection * dashSpeed;

        if (Time.time > dashEndTime)
        {
            is_dashing = false;
            rb.velocity = Vector2.zero; 
        }
    }

    void parry_animation()
    {
        string str = "is_parry_perfect";
        if (isRightClick)
        {

                animator.SetBool(str, true);
                lastPlayTime = Time.time;
                StartCoroutine(ResetAnimation(str, 0.4f));
            isRightClick = false;
        }
    }

    void is_parry_still()
    {
        string str = "is_parry_still";
        if (isRightclickhold)
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays)
            {
                animator.SetBool(str, true);
                lastPlayTime = Time.time;
                StartCoroutine(ResetAnimation(str, 0f));
            }
            isRightclickhold = false;
        }
    }

    void normal_attack()
    {
        string str = "is_attack";
        if (isLeftClick)
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays)
            {
                if (targets.Count > 0)
                {
                    foreach (var target in targets)
                    {
                        StartCoroutine(AttackRoutine(target, 2));
                    }
                }
                animator.SetBool(str, true);
                lastPlayTime = Time.time;
                StartCoroutine(ResetAnimation(str, 0f));
            }
            isLeftClick = false;
        }
    }

    void rolling()
    {
        string str = "is_rolling";
        if (is_rolling)
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays)
            {
                animator.SetBool(str, true);
                lastPlayTime = Time.time;
                StartCoroutine(ResetAnimation(str, 0f));
            }
            is_rolling = false;
        }
    }

    void ultimate_1()
    {
        string str = "is_ultimate";
        if (is_ultimate)
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays)
            {
                animator.SetBool(str, true);
                lastPlayTime = Time.time;
                StartCoroutine(ResetAnimation(str, 1f));
                Invoke("StartDash", 0.6f);
            }
            is_ultimate = false;
        }
    }

    void healing()
    {
        string str = "is_healing";
        if (is_healing)
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays)
            {
                animator.SetBool(str, true);
                lastPlayTime = Time.time;
                StartCoroutine(ResetAnimation(str, 1.3f));
                playerhealth = targetObject.GetComponent<PlayerHealth>();
                playerhealth.heal_skill_c();
            }
            is_healing = false;
        }
    }

    void buff()
    {
        string str = "is_buff";
        if (is_buff)
        {
            if (Time.time - lastPlayTime >= delayBetweenPlays)
            {
                animator.SetBool(str, true);
                lastPlayTime = Time.time;
                StartCoroutine(ResetAnimation(str, 0.7f));
            }
            is_buff = false;
        }
    }



    IEnumerator ResetAnimation(string x, float y)
    {

        yield return new WaitForSeconds(y);
        animator.SetBool(x, false);
    }

    /// -----------------------------------------------------------------------------------------------
    private bool canAttack = true;
    private float hitDelay = 0f;
    private List<Collider2D> targets = new List<Collider2D>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stairs"))
        {
            isOnStair = true;
        }
        if (other.CompareTag("Monster"))
        {
            if (!targets.Contains(other))
            {
                targets.Add(other);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Stairs"))
        {
            isOnStair = false;
        }
        if (other.CompareTag("Monster"))
        {
            targets.Remove(other);
        }
    }

    void HitTarget(Collider2D target)
    {
        if (target != null && canAttack)
        {
            Vector2 goblin_direction = set_movement_direction(target.transform.position, true, true);
            try
            {
                target.GetComponent<Monster_health>().TakeDamage(damage, goblin_direction);

            }
            catch
            {
                Debug.Log("Error Collision is Destroy cause after enemy die i destroy collision because it fucking roughly");
            }
        }
    }

    IEnumerator AttackRoutine(Collider2D target, int hits)
    {
        if (target != null)
        {
            for (int i = 0; i < hits; i++)
            {
                HitTarget(target);
                yield return new WaitForSeconds(hitDelay);
            }
        }
    }
    Vector2 set_movement_direction(Vector2 targetPosition, bool isNone = false, bool wantMovement = true)
    {
        Vector2 movementDirection = Vector2.zero;

        if (targetPosition != Vector2.zero && !isNone)
        {
            movementDirection = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = movementDirection * 0f;
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
}
