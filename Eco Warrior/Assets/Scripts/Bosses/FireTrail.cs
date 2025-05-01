using System.Collections.Generic;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 2f; // Damage dealt to the player
    [SerializeField] private float damageCooldown = 0.5f; // Cooldown time between damage

    // Static dictionary to track the last damage time for each player
    private static readonly Dictionary<GameObject, float> playerDamageTimers = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamage(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamage(other);
        }
    }

    private void ApplyDamage(Collider2D playerCollider)
    {
        GameObject playerObject = playerCollider.gameObject;

        // Get the last damage time for this player
        if (!playerDamageTimers.TryGetValue(playerObject, out float lastDamageTime))
        {
            lastDamageTime = -Mathf.Infinity; // Default to a very old time if not found
        }

        // Check if enough time has passed since the last damage
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            // Locate the HealthbarBehavior component on the player or its children
            HealthbarBehavior playerHealth = playerObject.GetComponentInChildren<HealthbarBehavior>();
            if (playerHealth != null)
            {
                // Apply damage to the player through the health bar
                playerHealth.HitDamage(damage, gameObject);

                // Update the last damage time for this player
                playerDamageTimers[playerObject] = Time.time;

                Debug.Log($"Fire Trail damaged the player for {damage} HP at time {Time.time}.");
            }
            else
            {
                Debug.LogWarning("Player does not have a HealthbarBehavior component.");
            }
        }
        else
        {
            Debug.Log($"Damage skipped for player. Current time: {Time.time}, next allowed damage time: {lastDamageTime + damageCooldown}.");
        }
    }
}

