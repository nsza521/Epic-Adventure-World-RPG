using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab; // Reference to the Player prefab
    private GameObject playerInstance; // Instance of the player

    void Start()
    {
        // Check if playerPrefab is assigned
        if (playerPrefab != null)
        {
            // Spawn the player at the start of the game
            playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player prefab is not assigned in the GameManager.");
        }
    }

    void Update()
    {
        if (playerInstance != null)
        {
            PlayerHealth playerScript = playerInstance.GetComponent<PlayerHealth>();

            if (playerScript != null && playerScript.health < 100)
            {
                Destroy(playerInstance);
                Debug.Log("Player has died. Object destroyed.");
            }
        }
    }
}
