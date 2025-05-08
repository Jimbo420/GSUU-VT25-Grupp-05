using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTransition : MonoBehaviour
{
    public string nextSceneName; // Name of the next scene to load
    private TextMeshProUGUI interactionText; // Reference to the TextMeshPro text component
    private TextMeshProUGUI inactiveText;
    private bool isPlayerInTrigger = false; // Tracks if the player is in the trigger zone
    public bool isScriptActive = true; // Flag to enable or disable the script

    void Start()
    {
        // Get all TextMeshProUGUI components in the child objects
        TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();

        if (textComponents.Length > 0)
        {
            interactionText = textComponents[0]; // Assign the first one
        }

        if (textComponents.Length > 1)
        {
            inactiveText = textComponents[1]; // Assign the second one
        }

        // Ensure the interaction text is hidden at the start
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }

        if (inactiveText != null)
        {
            inactiveText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            if (!isScriptActive)
            {
                if (interactionText != null)
                {
                    inactiveText.gameObject.SetActive(true);
                }
            }
            else
            {
                if (interactionText != null)
                {
                    interactionText.gameObject.SetActive(true);
                }
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
                inactiveText.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Check if the player is in the trigger zone and presses the F key
        if (isPlayerInTrigger && isScriptActive && Input.GetKeyDown(KeyCode.F))
        {
            // Load the next scene
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
