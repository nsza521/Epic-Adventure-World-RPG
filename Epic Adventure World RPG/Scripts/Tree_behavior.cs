using UnityEngine;

public class Tree_behavior : MonoBehaviour
{
    private int layer_default;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        layer_default = spriteRenderer.sortingOrder;
    }

    private void Update()
    {
        Debug.Log(spriteRenderer.sortingOrder);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        SpriteRenderer sprite_other = other.GetComponent<SpriteRenderer>();
        if (sprite_other != null)
        {
            if (other.CompareTag("Player"))
            {
                int layer_now = sprite_other.sortingOrder;
                spriteRenderer.sortingOrder = layer_now + 1;
                Debug.LogWarning("OnTriggerEnter2D");
            }
        }
        else
        {
            Debug.LogWarning("Object entered doesn't have a SpriteRenderer component.");
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        SpriteRenderer sprite_other = other.GetComponent<SpriteRenderer>();
        if (sprite_other != null)
        {

            if (other.CompareTag("Player"))
            {
                spriteRenderer.sortingOrder = layer_default;
                Debug.LogWarning("OnTriggerExit2D");
            }
        }
        else
        {
            Debug.LogWarning("Object exited doesn't have a SpriteRenderer component.");
        }
    }
}
