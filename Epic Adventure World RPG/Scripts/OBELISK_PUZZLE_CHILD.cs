using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBELISK_PUZZLE_CHILD : MonoBehaviour
{
    public bool is_light = false;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("player enter");
        }
    }
}
