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
    [SerializeField] private AudioSource audioSource; // AudioSource to play the sound

    // Static variable to track the last time damage was applied globally
    private static float lastGlobalDamageTime = -Mathf.Infinity;

    private void Start()
    {
        // Destroy the fire trail after its lifetime expires
        Destroy(gameObject, lifetime);
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

        // Check if enough time has passed since the last global damage
        if (Time.time < lastGlobalDamageTime + damageInterval)
        {
            return; // Skip damage if the global cooldown hasn't elapsed
        }

        // Update the last global damage time
        lastGlobalDamageTime = Time.time;

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
        if (audioSource == null)
        {
            // Dynamically add an AudioSource if none exists
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Ensure it doesn't play on awake
        }

        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
        else
        {
            Debug.LogWarning("DamageSound is not assigned.");
        }
    }
}
