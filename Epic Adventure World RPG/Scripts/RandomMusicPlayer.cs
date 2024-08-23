using System.Collections;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    public AudioClip[] musicTracks; 
    public AudioSource audioSource; 
    public bool playMusic = true;   

    void Start()
    {
        audioSource.volume = 0.5f;
        if (musicTracks.Length > 0 && audioSource != null)
        {
            StartCoroutine(PlayRandomMusic());
        }
        else
        {
            Debug.LogWarning("No music tracks assigned or AudioSource missing!");
        }
    }

    IEnumerator PlayRandomMusic()
    {
        while (true)
        {
            if (playMusic)
            {
                int randomIndex = Random.Range(0, musicTracks.Length);
                audioSource.clip = musicTracks[randomIndex];
                audioSource.Play();

                yield return new WaitForSeconds(audioSource.clip.length);
            }
            else
            {
                audioSource.Stop();
                yield return null; 
            }
        }
    }

    public void ToggleMusic()
    {
        playMusic = !playMusic;
    }
}
