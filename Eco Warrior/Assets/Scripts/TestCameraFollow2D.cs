using UnityEngine;

public class TestCameraFollow2D : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public Vector3 offset;   // Offset between the camera and player

    void LateUpdate()
    {
        // Follow the player's position with the offset
        transform.position = player.position + offset;
    }
}
