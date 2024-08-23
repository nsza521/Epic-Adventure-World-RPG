using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public Image healthbar;
    private Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    private Color color_dmg = Color.red;
    private Vector2 Knockback_direction = Vector2.zero;
    private AnimatorStateInfo currentAnimationState;

    public int health;
    public int max_health = 400;
    public bool enable_move = true;
    private float knockbackForce = 10f;
    public static PlayerHealth instance;
    public bool is_right_click = false;

    private float parryDuration = 0.5f;
    private bool isParrying = false;
    private float parryTimeCounter = 0f;

    void Start()
    {
        health = max_health;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void FixedUpdate()
    {
        float healthfloat = health/4;
        healthbar.fillAmount = healthfloat/100f;
        currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);
        if (isParrying)
        {
            parryTimeCounter += Time.deltaTime;
            if (parryTimeCounter >= parryDuration)
            {
                parryTimeCounter = 0f;
                isParrying = false;
            }
        }
        // Debug.Log(isParrying);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }
        if (is_right_click)
        {
            isParrying = true;
        }
        check_left_click();
    }
    public bool takedamage = false;
    public void TakeDamage(int damage, Vector2 monsterDirection)
    {
        takedamage = true;
        if (isParrying && (currentAnimationState.IsName("parry") || currentAnimationState.IsName("parrying")))
        {
            // Debug.Log("Parry");
            Knockback_direction = monsterDirection;
            StartCoroutine(Knockback());
        }
        else
        {
            if (currentAnimationState.IsName("roll"))
            {
                // Debug.Log("Dodge");
            }
            else
            {
                Debug.Log(health);
                // Debug.Log("get_hit");
                health -= damage;
                Knockback_direction = monsterDirection;
                StartCoroutine(Knockback());
                StartCoroutine(FlashDamageColor());
            }
        }
    }

    IEnumerator Knockback()
    {
        yield return new WaitForSeconds(0.3f);
        enable_move = false;
        rb.velocity = Knockback_direction.normalized * knockbackForce;
        yield return new WaitForSeconds(0.1f);
        enable_move = true;
        rb.velocity = Vector2.zero;
    }

    IEnumerator FlashDamageColor()
    {
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = color_dmg;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        takedamage = false;
    }

    void Die()
    {
        animator.SetTrigger("is_die");
        Invoke("hold_die",1.5f);
    }
    void hold_die()
    {
        animator.SetBool("just_die",true);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    void check_left_click()
    {
        is_right_click = Input.GetMouseButtonDown(1);
    }
    public void heal_skill_c()
    {
        if(health > 0)
        {
            health += 50;
        }
        if (health > 400) {
            health = 400;
        }
    }

}
