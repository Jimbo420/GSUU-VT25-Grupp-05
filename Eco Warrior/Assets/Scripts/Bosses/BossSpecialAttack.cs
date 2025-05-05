using UnityEngine;

public abstract class BossSpecialAttack : MonoBehaviour
{
    [Header("Special Attack Settings")]
    [Tooltip("Cooldown time between special attacks (in seconds).")]
    public float cooldown = 5f;

    private bool isOnCooldown = false;

    /// <summary>
    /// Executes the special attack logic.
    /// </summary>
    public void Execute()
    {
        if (isOnCooldown)
        {
            Debug.LogWarning("Special attack is on cooldown.");
            return;
        }

        PerformAttack();
        StartCoroutine(StartCooldown());
    }

    /// <summary>
    /// The specific logic for the special attack (to be implemented by subclasses).
    /// </summary>
    protected abstract void PerformAttack();

    /// <summary>
    /// Starts the cooldown timer for the special attack.
    /// </summary>
    private System.Collections.IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}

