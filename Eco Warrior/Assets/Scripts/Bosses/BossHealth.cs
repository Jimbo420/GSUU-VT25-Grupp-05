using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged HealthChanged;

    void Awake()
    {
        InitializeHealth();
    }

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth); // Notify listeners
    }

    public void TakeDamage(float damage)
    {
        //Debug.Log($"TakeDamage called with damage: {damage}");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0

        //Debug.Log($"Current health after damage: {currentHealth}");

        // Add a debug log to confirm the event is being invoked
        if (HealthChanged != null)
        {
            //Debug.Log("Invoking HealthChanged event.");
            HealthChanged.Invoke(currentHealth, maxHealth);
        }
        else
        {
            //Debug.LogWarning("HealthChanged event has no listeners.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Debug.Log("Boss has died.");
        Destroy(gameObject);
        SceneManager.LoadScene("ScoreScene");
    }
}




