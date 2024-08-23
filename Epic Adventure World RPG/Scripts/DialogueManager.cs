using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public Image dialog_box_actor_image;
    public TextMeshProUGUI dialog_box_actor_name;
    public TextMeshProUGUI dialog_box_actor_message;
    public RectTransform background_box;
    public AudioSource audioSource;

    public bool is_active = false;
    Message[] current_message;
    Actor[] current_actor;
    AudioClip[] current_audio_clips;

    int active_message = 0;
    
    public void Opendialogue(Message[] message, Actor[] actor,AudioClip[] voiceline)
    {
        current_message = message;
        current_actor = actor;
        current_audio_clips = voiceline;
        active_message = 0;

        is_active = true;

        Debug.Log("start"+message.Length);
        DisplayMessage();
    }
    void DisplayMessage()
    {
        Message messageToDisplay = current_message[active_message];
        dialog_box_actor_message.text = messageToDisplay.message;

        Actor actorToDisplay = current_actor[messageToDisplay.actor_id];
        dialog_box_actor_name.text = actorToDisplay.name;
        dialog_box_actor_image.sprite = actorToDisplay.sprite;

        AudioClip voiceline = current_audio_clips[active_message];
        audioSource.PlayOneShot(voiceline);
    }
    void NextMessage()
    {
        active_message++;
        if (active_message < current_message.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("Conversation End");
            is_active = false;
        }
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space) && is_active)
        {
            NextMessage();
        }
    }
}
