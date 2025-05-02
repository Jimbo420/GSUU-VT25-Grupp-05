using UnityEngine;

public class GasCanBehavior : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect; // Optional explosion effect

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the overlapping object is a bullet
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            TriggerExplosion();
            Destroy(gameObject); // Destroy the gas can
            Destroy(other.gameObject); // Destroy the bullet
        }
    }

    private void TriggerExplosion()
    {
        // Instantiate the explosion effect if one is assigned
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Gas can exploded!");
    }
}
