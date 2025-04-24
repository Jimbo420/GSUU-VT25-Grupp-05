using UnityEngine;
using TMPro;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public Sprite openDoorSprite; // Sprite for open door
    public TextMeshPro textDisplay; // Reference to the 3D TextMeshPro object
    public bool isKeyPickedUp = false; // Tracks if the key has been picked up
    private bool isDoorOpen = false; // Tracks if the door is open

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textDisplay.alpha = 0; // Text starts hidden
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the trigger
        {
            if (isKeyPickedUp && !isDoorOpen)
            {
                OpenDoor(); // Unlock the door if the key has been picked up
                ShowText("Unlocked");
                isDoorOpen = true; // Mark the door as open
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
}
