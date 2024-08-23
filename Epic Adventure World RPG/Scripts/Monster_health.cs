using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_health : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    private Color color_dmg = Color.red;
    private Vector2 Knockback_direction = Vector2.zero;

    public int health;
    public float time_hold_die;
    public bool enable_move = true;
    private float knockbackForce = 10f;
    public static Monster_health instance;
    public bool is_right_click = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        instance = this;
    }
    private bool one_hit = true;
    private void Update()
    {
        if (health <= 0 && one_hit)
        {
            Invoke("not_die_yet", 0.3f);
        }
    }
    private void not_die_yet()
    {
        Die();
        one_hit = false;
    }
    public void TakeDamage(int damage, Vector2 player_direction)
    {
        Debug.Log(health);
        health -= damage;
        Knockback_direction = player_direction;
        StartCoroutine(Knockback());
        StartCoroutine(FlashDamageColor());
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
    }

    void Die()
    {
        animator.SetBool("is_death",true);
        Invoke("hold_die", time_hold_die);
    }
    void hold_die()
    {
        DestroyGameObject();
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
    