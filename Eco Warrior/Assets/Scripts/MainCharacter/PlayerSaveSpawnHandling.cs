using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaveSpawnHandling : MonoBehaviour
{
    private static PlayerSaveSpawnHandling _instance;

    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // Persist this GameObject across scenes
    }

    private void Start()
    {
        MoveToPlayerStart(); // Move to "PlayerStart" on initial load

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MoveToPlayerStart(); // Move to "PlayerStart" when a new scene is loaded
    }

    private void MoveToPlayerStart()
    {
        // Find the GameObject with the "PlayerStart" tag
        GameObject playerStart = GameObject.FindGameObjectWithTag("PlayerStart");
        if (playerStart != null)
        {
            // Move this GameObject to the position of the "PlayerStart" GameObject
            transform.position = playerStart.transform.position;
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'PlayerStart' found in the scene.");
        }
    }
}
