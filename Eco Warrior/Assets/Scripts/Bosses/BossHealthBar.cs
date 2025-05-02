using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [Tooltip("Reference to the BossHealth component.")]
    public BossHealth bossHealth;
    [Tooltip("Foreground image representing the current health.")]
    public Image foregroundImage;
    [Tooltip("Optional: Text label for the boss's name.")]
    public TMPro.TextMeshProUGUI bossNameText;
    [Tooltip("The name of the boss.")]
    public string bossName = "Boss Name";

    void Start()
    {
        if (bossHealth == null)
        {
            Debug.LogError("BossHealth reference is not assigned in BossHealthBar.");
            return;
        }

        if (foregroundImage == null)
        {
            Debug.LogError("ForegroundImage reference is not assigned in BossHealthBar.");
            return;
        }

        // Set the boss name text
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }

        // Initialize the health bar
        UpdateHealthBar();
    }

    void OnEnable()
    {
        // Subscribe to the health change event
        if (bossHealth != null)
        {
            bossHealth.HealthChanged += OnHealthChanged;
        }
    }

    void OnDisable()
    {
        // Unsubscribe from the health change event
        if (bossHealth != null)
        {
            bossHealth.HealthChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        Debug.Log($"HealthChanged event triggered. Current Health: {currentHealth}, Max Health: {maxHealth}");
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (bossHealth != null && foregroundImage != null)
        {
            // Update the fill amount of the foreground image
            foregroundImage.fillAmount = bossHealth.currentHealth / bossHealth.maxHealth;

            // Debug log to confirm the fill amount
            Debug.Log($"Updated health bar fill amount: {foregroundImage.fillAmount}");

            // Update the boss name or health percentage (optional)
            if (bossNameText != null)
            {
                bossNameText.text = $"{bossName}"; // Display only the boss name
            }
        }
    }
}

