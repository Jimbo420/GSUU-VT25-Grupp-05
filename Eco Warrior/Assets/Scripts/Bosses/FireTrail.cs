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
    PlayerHealth _playerHealth;
    private static readonly Dictionary<GameObject, float> globalDamageTimers = new();
    private readonly Dictionary<GameObject, Coroutine> activeCoroutines = new();

    private void Awake()
    {
        // Clear the static dictionary when the game starts
        globalDamageTimers.Clear();
    }

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
        if (IsValidTarget(other.gameObject))
        {
            InitializeDamageTimer(other.gameObject);
            if (!activeCoroutines.ContainsKey(other.gameObject))
            {
                Coroutine coroutine = StartCoroutine(ApplyPeriodicDamage(other.gameObject));
                activeCoroutines[other.gameObject] = coroutine;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidTarget(other.gameObject) && activeCoroutines.ContainsKey(other.gameObject))
        {
            StopCoroutine(activeCoroutines[other.gameObject]);
            activeCoroutines.Remove(other.gameObject);
        }
    }

    private bool IsValidTarget(GameObject target)
    {
        // Ensure the target is valid (e.g., not the boss itself)
        return target.CompareTag("Player");
    }

    private void InitializeDamageTimer(GameObject target)
    {
        if (!globalDamageTimers.ContainsKey(target))
        {
            globalDamageTimers[target] = -Mathf.Infinity; // Set to a very old time
        }
    }

    private IEnumerator ApplyPeriodicDamage(GameObject target)
    {
        // Apply damage immediately
        ApplyDamage(target);

        while (true)
        {
            // Wait for the next damage interval
            yield return new WaitForSeconds(damageInterval);

            // Apply damage periodically
            ApplyDamage(target);
        }
    }

    private void ApplyDamage(GameObject target)
    {
        // Check if enough time has passed since the last damage for this target
        if (Time.time >= globalDamageTimers[target] + damageInterval)
        {
            // Update the last damage time for this target
            globalDamageTimers[target] = Time.time;

            // Attempt to apply damage through HealthbarBehavior
            HealthbarBehavior healthbar = target.GetComponentInChildren<HealthbarBehavior>();
            if (healthbar != null)
            {
                healthbar.HitDamage(damage, target);
                PlayDamageSound();
                //Debug.Log($"Fire Trail damaged {target.name} for {damage} HP.");
            }
            else
            {
                //Debug.LogWarning($"Fire Trail hit {target.name}, but it does not have a HealthbarBehavior.");
            }
        }
    }

    private void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
        else if (damageSound == null)
        {
            //Debug.LogWarning("DamageSound is not assigned.");
        }
    }
}


