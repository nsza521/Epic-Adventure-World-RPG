using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DialogTrigger : MonoBehaviour
{
    public Message[] messages;
    public Actor[] actor;
    public AudioClip[] voiceline_JP;
    public AudioClip[] voiceline_EN;
    public bool is_eng;


    public void StartDialogue()
    {
        if (is_eng)
        {
            FindAnyObjectByType<DialogueManager>().Opendialogue(messages, actor, voiceline_EN);
        }
        else
        {
            FindAnyObjectByType<DialogueManager>().Opendialogue(messages, actor, voiceline_JP);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered: " + other.gameObject.name);
            StartDialogue();
        }
    }
}

[System.Serializable]
public class Message
{
    public int actor_id;
    public string message;
}
[System.Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}
