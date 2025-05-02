using UnityEngine;

public class KeyID : MonoBehaviour
{
    public GameObject linkedDoor; // Reference to the door this key unlocks

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Key picked up!");
            linkedDoor.GetComponent<DoorScript>().isKeyPickedUp = true; // Mark the key as picked up
            Destroy(gameObject); // Remove the key from the scene
        }
    }
}
