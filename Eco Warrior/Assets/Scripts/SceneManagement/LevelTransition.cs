using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTransition : MonoBehaviour
{
    public string nextSceneName; // Name of the next scene to load
    private TextMeshProUGUI interactionText; // Reference to the TextMeshPro text component
    private bool isPlayerInTrigger = false; // Tracks if the player is in the trigger zone
    public bool isScriptActive = true; // Flag to enable or disable the script

    void Start()
    {
        // Automatically find the TextMeshProUGUI component in the child objects
        interactionText = GetComponentInChildren<TextMeshProUGUI>();

        // Ensure the interaction text is hidden at the start
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Interaction text not found! Make sure a TextMeshProUGUI component exists as a child of this GameObject.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            // Show the interaction text
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exited the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            // Hide the interaction text
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Check if the player is in the trigger zone and presses the F key
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            // Load the next scene
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
