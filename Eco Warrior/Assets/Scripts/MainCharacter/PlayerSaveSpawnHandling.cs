using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
        // Start a coroutine to delay the move to "PlayerStart"
        StartCoroutine(DelayedMoveToPlayerStart());
    }

    private IEnumerator DelayedMoveToPlayerStart()
    {
        // Wait for the end of the frame to ensure all GameObjects are initialized
        yield return new WaitForEndOfFrame();

        // Optionally, wait a bit longer if necessary
        yield return new WaitForSeconds(0.1f);

        MoveToPlayerStart(); // Move to "PlayerStart" after the delay
    }

    private void MoveToPlayerStart()
    {
        Debug.Log($"[PlayerSaveSpawnHandling] Current position before move: {transform.position}");

        // Find the GameObject with the "PlayerStart" tag
        GameObject playerStart = GameObject.FindGameObjectWithTag("PlayerStart");
        if (playerStart != null)
        {
            // Move this GameObject to the position of the "PlayerStart" GameObject
            transform.position = playerStart.transform.position;
            Debug.Log($"[PlayerSaveSpawnHandling] Moved to PlayerStart at position: {playerStart.transform.position}");
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'PlayerStart' found in the scene.");
        }
    }
}
