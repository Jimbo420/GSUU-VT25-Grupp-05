using UnityEngine;

public class DoorLockerZone : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("The door to lock when the player enters this zone.")]
    public GameObject door; // Reference to the door GameObject

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the trigger
        {
            if (door != null) // Ensure a door is assigned
            {
                DoorScript doorScript = door.GetComponent<DoorScript>();
                if (doorScript != null)
                {
                    doorScript.isDoorOpen = false; // Lock the door
                    doorScript.isKeyPickedUp = false; // Reset the key collected state
                    doorScript.CloseDoor(); // Explicitly call CloseDoor to update the door's state
                    Debug.Log("Door locked by DoorLockerZone!");
                }
                else
                {
                    Debug.LogWarning("The assigned door does not have a DoorScript component!");
                }
            }
            else
            {
                Debug.LogWarning("No door assigned to the DoorLockerZone!");
            }
        }
    }
}
