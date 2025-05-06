using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 2f; // Damage dealt to the player
    [SerializeField] private float damageInterval = 0.5f; // Time interval between damage applications
    [SerializeField] private float lifetime = 5f; // Lifetime of the fire trail before it disappears

    [Header("Audio Settings")]
    [SerializeField] private AudioClip damageSound; // Sound effect for damage
    private AudioSource audioSource; // Dynamically added AudioSource

    // Static dictionary to track the last time damage was applied globally for each target
    private static readonly Dictionary<GameObject, float> globalDamageTimers = new();

    private void Start()
    {
        // Destroy the fire trail after its lifetime expires
        Destroy(gameObject, lifetime);

        // Dynamically add an AudioSource if it doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Ensure it doesn't play on awake
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplyDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ApplyDamage(other);
    }

    private void ApplyDamage(Collider2D other)
    {
        GameObject target = other.gameObject;

        // Ensure the target is valid (e.g., not the boss itself)
        if (!target.CompareTag("Player"))
        {
            return; // Skip if the target is not the player
        }

        // Check if enough time has passed since the last damage for this target globally
        if (!globalDamageTimers.TryGetValue(target, out float lastDamageTime))
        {
            lastDamageTime = -Mathf.Infinity; // Default to a very old time if not found
        }

        if (Time.time < lastDamageTime + damageInterval)
        {
            return; // Skip damage if the cooldown hasn't elapsed for this target
        }

        // Update the last damage time for this target globally
        globalDamageTimers[target] = Time.time;

        // Attempt to apply damage through HealthbarBehavior
        HealthbarBehavior healthbar = target.GetComponentInChildren<HealthbarBehavior>();
        if (healthbar != null)
        {
            healthbar.HitDamage(damage, target);
            PlayDamageSound();
            Debug.Log($"Fire Trail damaged {target.name} for {damage} HP.");
            return;
        }

        // If no HealthbarBehavior is found, log a warning
        Debug.LogWarning($"Fire Trail hit {target.name}, but it does not have a HealthbarBehavior.");
    }

    private void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
        else if (damageSound == null)
        {
            Debug.LogWarning("DamageSound is not assigned.");
        }
    }
}

