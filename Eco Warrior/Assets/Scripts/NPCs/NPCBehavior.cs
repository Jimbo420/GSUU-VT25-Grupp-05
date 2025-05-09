using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Animator animator; // Reference to the NPC's Animator

    [Header("Chat Bubble Settings")]
    [Tooltip("Prefab for the chat bubble.")]
    public GameObject chatBubblePrefab;
    [Tooltip("Parent transform for the chat bubble.")]
    public Transform chatBubbleParent;

    [Header("Chat Bubble Text and Duration")]
    [Tooltip("List of chat bubble messages and their durations.")]
    public List<ChatBubbleData> chatBubbleData;

    [Header("Special Return Text")]
    [Tooltip("Special text to display when the player returns after all dialogues are finished.")]
    public string returnText;

    private bool isPlayerInRange = false; // Tracks if the player is in range
    private bool hasFinishedDialogues = false; // Tracks if all dialogues have been shown
    private GameObject activeChatBubble; // Reference to the currently active chat bubble
    private Coroutine chatBubbleCoroutine; // Reference to the chat bubble coroutine

    void Update()
    {
        if (isPlayerInRange)
        {
            // Calculate the direction vector
            Vector3 direction = (player.position - transform.position).normalized;

            // Debugging: Log the direction vector
            Debug.Log($"Direction: {direction}");

            // Determine the IdleDirection based on the direction vector
            int idleDirection;
            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                // Prioritize vertical movement
                idleDirection = direction.y > 0 ? 1 : 0; // 1 = up, 0 = down
            }
            else
            {
                // Horizontal movement
                idleDirection = direction.x > 0 ? 3 : 2; // 3 = right, 2 = left
            }

            // Debugging: Log the chosen IdleDirection
            Debug.Log($"IdleDirection: {idleDirection}");

            // Update the animator parameter
            animator.SetInteger("IdleDirection", idleDirection);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Start showing chat bubbles or return text
            if (hasFinishedDialogues)
            {
                ShowChatBubble(returnText); // Show the return text as long as the player is in the trigger
            }
            else if (chatBubbleCoroutine == null)
            {
                chatBubbleCoroutine = StartCoroutine(ShowChatBubbles());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Hide the chat bubble when the player leaves the trigger
            HideChatBubble();
        }
    }

    private IEnumerator ShowChatBubbles()
    {
        foreach (var bubbleData in chatBubbleData)
        {
            ShowChatBubble(bubbleData.text);

            // Wait for the specified duration
            yield return new WaitForSeconds(bubbleData.duration);

            HideChatBubble();
        }

        // Mark dialogues as finished
        hasFinishedDialogues = true;

        // Reset the coroutine reference when done
        chatBubbleCoroutine = null;

        // Show the return text if the player is still in range
        if (isPlayerInRange)
        {
            ShowChatBubble(returnText);
        }
    }

    private void ShowChatBubble(string message)
    {
        // Destroy the old chat bubble if it exists
        if (activeChatBubble != null)
        {
            Destroy(activeChatBubble);
            activeChatBubble = null;
        }

        if (chatBubblePrefab == null || chatBubbleParent == null)
        {
            Debug.LogWarning("Chat bubble prefab or parent is not assigned.");
            return;
        }

        // Instantiate the new chat bubble
        activeChatBubble = Instantiate(chatBubblePrefab, chatBubbleParent);
        var textComponent = activeChatBubble.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
        }
    }

    private void HideChatBubble()
    {
        if (activeChatBubble != null)
        {
            Destroy(activeChatBubble);
            activeChatBubble = null;
        }
    }
}

[System.Serializable]
public class ChatBubbleData
{
    [Tooltip("The text to display in the chat bubble.")]
    public string text;
    [Tooltip("The duration (in seconds) to display the chat bubble.")]
    public float duration;
}
