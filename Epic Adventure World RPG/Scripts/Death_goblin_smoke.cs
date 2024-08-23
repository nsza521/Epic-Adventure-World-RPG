using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_goblin_smoke : MonoBehaviour
{
    public Animator animator_goblin;
    public Animator animator_smoke;
    public string deathAnimationStateName = "death";
    private bool isDead = false;

    void Start()
    {
        animator_smoke = GetComponent<Animator>();
        animator_goblin = GameObject.Find("goblin").GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator_goblin.GetCurrentAnimatorStateInfo(0);
        bool isDeathAnimationPlaying = stateInfo.IsName(deathAnimationStateName);

        if (isDeathAnimationPlaying && !isDead)
        {
            isDead = true;
            Invoke("smoke_it", 1.15f);
        }
    }

    void smoke_it()
    {
        animator_smoke.SetTrigger("smoke_it");
    }
}
