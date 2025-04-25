using UnityEngine;

public class FireTrail : MonoBehaviour
{
    [SerializeField] private float damage = 5f; // Damage dealt to the player
    [SerializeField] private float damageCooldown = 1f; // Cooldown time between damage
    private float lastDamageTime = -Mathf.Infinity; // Tracks the last time damage was applied

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

    private void ApplyDamage(Collider2D player)
    {
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            HealthbarBehavior playerHealth = player.GetComponentInChildren<HealthbarBehavior>();
            if (playerHealth != null)
            {
                // Pass the player's GameObject to HitDamage
                playerHealth.HitDamage(damage, player.gameObject); // Fire applies damage to player
                lastDamageTime = Time.time; // Update the last damage time

                Debug.Log($"Fire Trail damaged the player for {damage} HP. Checking if entity is destroyed.");
            }
        }
    }
}
