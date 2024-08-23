using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tp_stone : MonoBehaviour
{
    public float detectionRange = 5f;

    public LayerMask playerLayer;
    public Animator animator;

    private bool is_enter = false;

    private void FixedUpdate()
    {
        if (is_enter)
        {
            animator.SetBool("is_player_nearby", true);
        }
        else
        {
            animator.SetBool("is_player_nearby", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered: ");
            is_enter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exited");
            is_enter = false;
        }
    }

}
