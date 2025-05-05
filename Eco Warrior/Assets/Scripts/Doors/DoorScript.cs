using UnityEngine;
using TMPro;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Sprite for the open door.")]
    public Sprite openDoorSprite; // Sprite for open door
    [Tooltip("Sprite for the closed door.")]
    public Sprite closedDoorSprite; // Sprite for closed door
    [Tooltip("Reference to the 3D TextMeshPro object.")]
    public TextMeshPro textDisplay; // Reference to the 3D TextMeshPro object
    [Tooltip("Tracks if the key has been picked up.")]
    public bool isKeyPickedUp = false; // Tracks if the key has been picked up
    [Tooltip("Tracks if the door is open.")]
    public bool isDoorOpen = false; // Tracks if the door is open

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textDisplay.alpha = 0; // Text starts hidden

        // Check if the door should start open
        if (isDoorOpen)
        {
            OpenDoor(); // Open the door immediately if the flag is set
        }
        else
        {
            CloseDoor(); // Ensure the door starts closed if not open
        }
    }

    void Update()
    {
        // Automatically open the door if the isDoorOpen flag is set in the Inspector
        if (isDoorOpen && spriteRenderer.sprite != openDoorSprite)
        {
            OpenDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the trigger
        {
            if (isKeyPickedUp && !isDoorOpen)
            {
                isDoorOpen = true; // Mark the door as open
                OpenDoor(); // Unlock the door if the key has been picked up
                ShowText("Unlocked");
            }
            else if (!isKeyPickedUp && !isDoorOpen) // Check if the key hasn't been picked up
            {
                ShowText("Locked"); // Show "Locked" text if the key hasn't been picked up
            }
        }
    }

    private void OpenDoor()
    {
        spriteRenderer.sprite = openDoorSprite; // Change the door sprite
        GetComponent<Collider2D>().enabled = false; // Disable the door collider
        Debug.Log("Door Unlocked!");
    }

    public void CloseDoor()
    {
        spriteRenderer.sprite = closedDoorSprite; // Change the door sprite to closed
        GetComponent<Collider2D>().enabled = true; // Enable the door collider
        Debug.Log("Door Locked!");
    }

    private void ShowText(string message)
    {
        textDisplay.text = message; // Update the text
        StartCoroutine(FadeText()); // Fade in and out the text
    }

    private IEnumerator FadeText()
    {
        float alpha = textDisplay.alpha;

        // Fade in text
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            textDisplay.alpha = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Keep text visible for 1 second

        // Fade out text
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            textDisplay.alpha = alpha;
            yield return null;
        }
    }

    // This method is called whenever a value is changed in the Inspector
    private void OnValidate()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        var collider = GetComponent<Collider2D>();

        if (isDoorOpen)
        {
            // Change the sprite to the open door sprite
            spriteRenderer.sprite = openDoorSprite;

            // Disable the collider in the editor
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
        else
        {
            // Change the sprite to the closed door sprite
            spriteRenderer.sprite = closedDoorSprite;

            // Re-enable the collider if the door is closed
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }
}
